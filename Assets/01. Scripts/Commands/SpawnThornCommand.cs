using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class SpawnThornCommand : AbstractCommand<SpawnThornCommand>
    {
        public Vector2 TargetPos;
        public Vector2 SpawnPosition;
        public GameObject BulletPrefab;
        public bool ShootInstant;
        public int Damage;

        public static SpawnThornCommand Allocate(Vector2 targetPos, Vector2 spawnPosition, GameObject BulletPrefab,
            bool ShootInstant, int Damage)
        {
            SpawnThornCommand cmd = SafeObjectPool<SpawnThornCommand>.Singleton.Allocate();
            cmd.TargetPos = targetPos;
            cmd.SpawnPosition = spawnPosition;
            cmd.BulletPrefab = BulletPrefab;
            cmd.ShootInstant = ShootInstant;
            cmd.Damage = Damage;
            return cmd;
        }
        protected override void OnExecute()
        {
            Thorn bullet = GameObject.Instantiate(BulletPrefab, SpawnPosition,
                Quaternion.identity).GetComponent<Thorn>();

           // Debug.Log(bullet.name);
            bullet.TargetPos = TargetPos;
            bullet.Damage = Damage;
            bullet.ShootInstant = ShootInstant;
        }
    }
}
