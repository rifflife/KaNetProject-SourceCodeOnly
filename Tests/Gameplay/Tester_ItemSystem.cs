using Gameplay;
using Gameplay.Legacy;
using KaNet.Synchronizers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utils.Analytics;

[Obsolete]
public class Tester_ItemSystem : MonoBehaviour
{
	[Test]
	public void Test_ItemTransformCollide()
	{
		ItemTransform item1 = new ItemTransform
		(
			new Int8Vector2(0, 0),
			new Int8Vector2(3, 1),
			false
		);

		ItemTransform item2 = new ItemTransform
		(
			new Int8Vector2(3, 0),
			new Int8Vector2(3, 1),
			false
		);

		Assert.IsFalse(item1.IsCollideWith(item2));
		Assert.IsFalse(item2.IsCollideWith(item1));

		item1.MoveTo((2, 0));

		Assert.IsTrue(item1.IsCollideWith(item2));
		Assert.IsTrue(item2.IsCollideWith(item1));

		item1.MoveTo((0, 0));
		item1.SetRotation(true);
		item2.MoveTo((0, 2));
		item2.SetRotation(false);

		Assert.IsTrue(item1.IsCollideWith(item2));
		Assert.IsTrue(item2.IsCollideWith(item1));

		item2.MoveTo((0, 3));

		Assert.IsFalse(item1.IsCollideWith(item2));
		Assert.IsFalse(item2.IsCollideWith(item1));
	}

	[Test]
	public void Test_ItemStack()
	{
		Item ammo_rifle_1 = new Item(ItemType.Ammo_Rifle_127, 20, 50);
		Item ammo_rifle_2 = new Item(ItemType.Ammo_Rifle_127, 40, 50);

		Assert.IsTrue(ammo_rifle_1.TryStack(ammo_rifle_2));
		Assert.AreEqual(ammo_rifle_1.Count, ammo_rifle_1.MaxStack);
		Assert.AreEqual(ammo_rifle_2.Count, (NetUInt8)10);

		Item ammo_rifle_3 = new Item(ItemType.Ammo_Rifle_127, 0, 50);
		ammo_rifle_3.TryStack(ammo_rifle_2);
		Assert.AreEqual(ammo_rifle_2.Count, (NetUInt8)0);
		Assert.AreEqual(ammo_rifle_3.Count, (NetUInt8)10);
	}

	[Test]
	public void Test_InventroyAddOrRemove()
	{
		Inventory inventory = new Inventory((2, 2));

		for (int i = 0; i < 4; i++)
		{
			Item ammo = new Item(ItemType.Ammo_Rifle_127, 50, 50);
			Assert.IsTrue(inventory.TryAdd(ammo) == InventoryOperationResult.Success);
			Assert.AreEqual(i + 1, inventory.Count);
		}

		Item ammo1 = new Item(ItemType.Ammo_Rifle_127, 50, 50);
		Assert.IsTrue(inventory.TryAdd(ammo1) == InventoryOperationResult.ThereIsNoSpace);
		Assert.IsFalse(inventory.HasSpace(Int8Vector2.One, out var dpos));

		int count = 3;
		foreach (var item in inventory.Items)
		{
			Assert.IsTrue(inventory.TryRemove(item) == InventoryOperationResult.Success);
			Assert.AreEqual(count, inventory.Count);
			count--;
		}
	}

	[Test]
	public void Test_InventroyStackItem()
	{
		Inventory inventory = new Inventory((3, 3));

		int itemCount = 15;
		int repeatCount = 8;
		byte maxCount = 50;

		for (int i = 0; i < repeatCount; i++)
		{
			Item ammo = new Item(ItemType.Ammo_Rifle_556, (byte)itemCount, maxCount);
			Assert.IsTrue(inventory.TryAdd(ammo) == InventoryOperationResult.Success);
		}

		WeaponBase weapon_1 = new WeaponBase(ItemType.Weapon_Rifle_1, (3, 1), 0.1f, new WeaponItemInfo());

		inventory.TryAdd(weapon_1);
		Assert.AreEqual(inventory.GetItemCountBy(ItemType.Weapon_Rifle_1), 1);

		WeaponBase weapon_2 = new WeaponBase(ItemType.Weapon_Rifle_1, (3, 1), 0.1f, new WeaponItemInfo());
		inventory.TryAdd(weapon_2);
		Assert.AreEqual(inventory.GetItemCountBy(ItemType.Weapon_Rifle_1), 2);

		int ammoCount = inventory.GetItemCountBy(ItemType.Ammo_Rifle_556);
		int totalAmmo = itemCount * repeatCount;
		Assert.AreEqual(totalAmmo, ammoCount);
		Assert.AreEqual
		(
			inventory.GetItemBundleCountBy(ItemType.Ammo_Rifle_556),
			(int)Mathf.Ceil(totalAmmo / (float)maxCount)
		);

		Assert.IsTrue(inventory.TryRemoveBy(ItemType.Ammo_Rifle_556, 50) == InventoryOperationResult.Success);
		totalAmmo -= 50;
		Assert.AreEqual(totalAmmo, inventory.GetItemCountBy(ItemType.Ammo_Rifle_556));
		Assert.AreEqual
		(
			inventory.GetItemBundleCountBy(ItemType.Ammo_Rifle_556),
			(int)Mathf.Ceil(totalAmmo / (float)maxCount)
		);

		Assert.IsTrue(inventory.TryRemoveBy(ItemType.Ammo_Rifle_556, 50) == InventoryOperationResult.Success);
		totalAmmo -= 50;
		Assert.AreEqual(totalAmmo, inventory.GetItemCountBy(ItemType.Ammo_Rifle_556));
		Assert.AreEqual
		(
			inventory.GetItemBundleCountBy(ItemType.Ammo_Rifle_556),
			(int)Mathf.Ceil(totalAmmo / (float)maxCount)
		);
	}

	[Test]
	public void Test_InventoryWeaponSlot()
	{
		//Inventory inventory = new Inventory((5, 5));

		WeaponBase weapon_1 = new WeaponBase(ItemType.Weapon_Rifle_1, (3, 1), 0.1f, new WeaponItemInfo());
		WeaponBase weapon_2 = new WeaponBase(ItemType.Weapon_Pistol_1, (3, 1), 0.1f, new WeaponItemInfo());

		UserLoadoutData userData = new UserLoadoutData((2, 3));

		Assert.IsTrue(userData.Inventory.TryAdd(weapon_1) == InventoryOperationResult.Success);
		Assert.IsTrue(userData.Inventory.TryAdd(weapon_2) == InventoryOperationResult.Success);

		Assert.IsTrue(userData.TryBindWeaponSlot(0, weapon_1) == InventoryOperationResult.Success);
		Assert.IsTrue(userData.TryBindWeaponSlot(0, weapon_2) == InventoryOperationResult.Success);
	}
}
