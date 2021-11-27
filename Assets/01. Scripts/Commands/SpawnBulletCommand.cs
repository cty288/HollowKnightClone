using System.Collections;
using System.Collections.Generic;
using MikroFramework.Architecture;
using MikroFramework.Pool;
using UnityEngine;

namespace HollowKnight
{
    public class SpawnBulletCommand : AbstractCommand<SpawnBulletCommand> {
        public GameObject Target;
        public Transform SpawnPosition;
        public GameObject BulletPrefab;
        public bool ShootInstant;
        public int Damage;

        public static SpawnBulletCommand Allocate(GameObject target, Transform spawnPosition, GameObject BulletPrefab,
            bool ShootInstant, int Damage) {
            SpawnBulletCommand cmd = SafeObjectPool<SpawnBulletCommand>.Singleton.Allocate();
            cmd.Target = target;
            cmd.SpawnPosition = spawnPosition;
            cmd.BulletPrefab = BulletPrefab;
            cmd.ShootInstant = ShootInstant;
            cmd.Damage = Damage;
            return cmd;
        }
        protected override void OnExecute() {
            Bullet bullet = GameObject.Instantiate(BulletPrefab, SpawnPosition.position,
                Quaternion.identity).GetComponent<Bullet>();

            bullet.Target = Target.transform;
            bullet.Damage = Damage;
            bullet.ShootInstant = ShootInstant;
        }
    }
}
