using KdTree;
using KdTree.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarioWorldSharp.Levels;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace MarioWorldSharp.Entities
{
    public class SpawnArgs : EventArgs
    {
        public IEntity[] SpawnArr;
    }
    public class EntityHandler
    {
        private static KdTree<double, IEntity> EntitiesTree = new KdTree<double, IEntity>(2, new DoubleMath());
        public static List<IEntity> EntityList { get; set; }
        public static IEntity EntityLastSpawned { get; set; }
        public static IEntity EntityLastDepawned { get; set; }
        public static int UpdateCalls
        {
            get
            {
                if (!SMW.SecondPassed)
                    return _Updated2;

                _Updated2 = _Updated;
                return _Updated;
            }
        }
        public static int EntityCount { get; private set; }

        private static int _Updated;
        private static int _Updated2;

        public static async void ProcessEnteties()
        {
            if (SMW.SecondPassed)
            { _Updated = 0; }

            SMW.Level.SpawnEntitiesOnScroll();
            UpdateCollisionTree();
            if (EntityCount != 0) await Task.Run( () =>
            {
                foreach (IEntity e in EntityList.ToArray())
                {
                    if (e != null)
                    {
                        double[] oldPos = { e.XPosition, e.YPosition };
                        e.Process();
                        /*if ((s.Data.InteractWithSprites) &&
                            (s.XPosition != oldPos[0] || s.YPosition != oldPos[1] || s.Status == SpriteStatus.NonExistent))
                        { UpdateCollisionTree(s, oldPos, s.Status == SpriteStatus.NonExistent); _Updated++; }
                        else*/
                        if (e.Status == EntityStatus.NonExistent && EntityList.Remove(e))
                        {
                            EntityCount--;
                            EntityLastDepawned = e;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Returns the collision KdTree of the sprites on-screen. 
        /// Useful for searching for on-screen sprites by distance from each other
        /// </summary>
        /// <returns></returns>
        public static KdTree<double, IEntity> GetEntityTree()
        {
            return EntitiesTree;
        }

        public static IEntity[] GetNearestNeighbors(double[] point, int count)
        {
            KdTreeNode<double, IEntity>[] Entities = EntitiesTree.GetNearestNeighbours(point, count);
            IEntity[] ret = new IEntity[Entities.Length];
            int i = 0;
            foreach (KdTreeNode<double, IEntity> s in Entities)
            {
                if (i >= EntityCount)
                    break;

                ret[i] = s.Value;
                i++;
            }
            return ret;
        }

        public static async void UpdateCollisionTreeAsync()
        {
            await Task.Run(() => UpdateCollisionTree());
        }

        public static async void UpdateCollisionTreeAsync(IEntity s, double[] oldPos, bool remove)
        {
            await Task.Run(() => UpdateCollisionTree(s, oldPos, remove));
        }

        /// <summary>
        /// Update all the entities in the collision tree
        /// </summary>
        /// 
        public static void UpdateCollisionTree()
        {
            if (EntityCount == 0)
                return;

            EntitiesTree.Clear();

            foreach (IEntity e in EntityList.ToArray())
            {
                if (e != null)
                    if (e.Data.InteractWithEntities)
                        EntitiesTree.Add(new[] { e.XPosition, e.XPosition }, e);
            }

            if (EntityCount > 0)
                EntitiesTree.Balance();
            _Updated++;
        }

        public static void KillEntites(object sender, EventArgs e)
        {
            foreach (IEntity s in EntityList.ToArray())
                s?.Kill();
        }

        private static void UpdateCollisionTree(IEntity e, double[] oldPos, bool remove)
        {
            EntitiesTree.RemoveSpecific(oldPos, e);

            if (!remove)
            { EntitiesTree.Add(new[] { e.XPosition, e.YPosition }, e); }
            else
            { EntityCount--; EntityList.Remove(e); EntityLastDepawned = e; }

            //if (Count != 0)
            //SpritesTree.Balance();
        }

        /// <summary>
        /// Adds the entity to the processing list
        /// </summary>
        /// <param name="entity"></param>
        public static void AddEntity(params IEntity[] entity)
        {
            foreach (IEntity e in entity)
            {
                if (EntityList == null)
                    EntityList = new List<IEntity>();
                if (e.Data.InteractWithEntities)
                    EntitiesTree.Add(new[] { e.XPosition, e.YPosition }, e);
                EntityList.Add(e);
                EntityCount++;
                EntityLastSpawned = e;
            }
        }
    }

    public enum EntityID
    {
        GreenShellessKoopa,
        Test
    }
    public class EntitySpawner
    {
        public static IEntity SpawnEntity(double[] point, EntityData d, params object[] args)
        {
            return SpawnEntity(point[0], point[1], d, args);
        }
        public static IEntity SpawnEntity(double x, double y, EntityData d, params object[] args)
        {
            switch (d.ID)
            {
                case EntityID.GreenShellessKoopa:
                    return new ShellessKoopa(x, y, d, 0);
                case EntityID.Test:
                    d.InteractWithEntities = false;
                    return new TestEntity(x, y, d);
                default:
                    return null;
            }
        }
    }

    public class UnlistedEntityException : Exception
    {
        public UnlistedEntityException(string message) : base(message)
        {
        }
    }
    public class UnindexedEntityException : Exception
    {
        public UnindexedEntityException(string message) : base(message)
        {
        }
    }
}
