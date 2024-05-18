using Godot;
using System;
using System.Linq;

public partial class SMGEffectController : Node
{
	[Export] private Node3D _barrelEnd;
	[Export] public RayCast3D BarrelRayCast;
	[Export] private PackedScene _muzzleFlash;
	[Export] private PackedScene _impactEffect;
	[Export] private PackedScene _bulletProjectile;
	[Export] private float _impactForce { get; set; } = 5f;
	[Export] private float _bulletSpeed { get; set; } = 5f;
	[Export] private int _maxAmmo { get; set; } = 15;

	public bool HasAmmo { get; private set; } = true;
	private int currentAmmo;

	public override void _Ready()
	{
		currentAmmo = _maxAmmo;

	}

	public void Reload()
	{
		currentAmmo = _maxAmmo;
		HasAmmo = true;
	}

	public void Fire()
	{
		Node3D newMuzzleFlash = _muzzleFlash.Instantiate() as Node3D;
		_barrelEnd.AddChild(newMuzzleFlash);
		newMuzzleFlash.GlobalPosition = _barrelEnd.GlobalPosition;
		newMuzzleFlash.GlobalRotation = _barrelEnd.GlobalRotation;

		GD.Print("fired");

		currentAmmo -= 1;
		HasAmmo = currentAmmo > 0;

		if (BarrelRayCast.IsColliding())
		{
			Vector3 hitLocation = BarrelRayCast.GetCollisionPoint();
			PackedScene damageableEffect = _impactEffect;
			Node3D impactTarget = BarrelRayCast.GetCollider() as Node3D;

			if (impactTarget is RigidBody3D rigidBody)
			{
				GD.Print("hit a" + rigidBody);
				rigidBody.ApplyImpulse(BarrelRayCast.GlobalTransform.Basis.Z * _impactForce, hitLocation - rigidBody.GlobalPosition);
			}
			// if the target is damageable invoke its' impact effect (crush it, damage it)
			if (impactTarget.GetChildren().FirstOrDefault(x => x.Name == "Damageable") is DamageableObject damageable)
			{
				if (damageable.HitObject != null)
				{
					damageableEffect = damageable.ImpactEffect;
				}
				damageable.HitObject(hitLocation, -BarrelRayCast.GlobalTransform.Basis.Z * _impactForce);
			}

			Node3D newImpactEffect = damageableEffect.Instantiate() as Node3D;
			AddChild(newImpactEffect);
			newImpactEffect.GlobalPosition = hitLocation;

			Vector3 lookatPoint = BarrelRayCast.GetCollisionNormal();
			if (lookatPoint != Vector3.Zero)
			{
				newImpactEffect.LookAt(newImpactEffect.GlobalPosition + lookatPoint, Mathf.Abs(lookatPoint.Y) > 0.99 ? Vector3.Back : Vector3.Up);
			}

			RigidBody3D newBullet = _bulletProjectile.Instantiate() as RigidBody3D;
			AddChild(newBullet);
			newBullet.GlobalPosition = _barrelEnd.GlobalPosition;
			newBullet.LinearVelocity = BarrelRayCast.GlobalTransform.Basis.Z * _bulletSpeed;
		}
	}

}
