using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using MidtermExam.Prob01;

namespace MidtermExam.Tests
{
    /// <summary>
    /// ========================================================================================
    /// ชุดแบบทดสอบ Problem 01: OOP & Class Diagram Implementation (50 คะแนน)
    /// ========================================================================================
    /// 
    /// วัตถุประสงค์:
    /// ตรวจสอบความถูกต้องของการแปลง Class Diagram เป็นโค้ด C# ทั้ง 11 คลาส
    /// 
    /// สิ่งที่แบบทดสอบนี้ตรวจสอบ:
    /// 1. [TC01] การมีอยู่ของคลาสทั้ง 11 คลาสใน namespace MidtermExam.Prob01
    /// 2. [TC02] โครงสร้างการสืบทอดคุณสมบัติ (Inheritance Hierarchy)
    /// 3. [TC03 - TC13] ตัวแปร (Fields), เมธอด (Methods), Access Modifiers (+, #, -),
    ///    ชนิดข้อมูล (Data Types) และการใช้ virtual / override ให้ตรงตามสเปก
    /// 
    /// สัญลักษณ์ Access Modifiers ใน Class Diagram:
    /// - เครื่องหมาย '+' หมายถึง public
    /// - เครื่องหมาย '#' หมายถึง protected
    /// - เครื่องหมาย '-' หมายถึง private
    /// ========================================================================================
    /// </summary>
    [TestFixture]
    [Category("MidtermExam")]
    [Category("Prob01")]
    public class Prob01_ClassDiagram_Testcase
    {
        private const string ExpectedNamespace = "MidtermExam.Prob01";

        // รายชื่อคลาสทั้ง 11 คลาสที่นักศึกษาต้องสร้างให้ครบ
        private readonly string[] expectedClasses = new string[]
        {
            "GameEntity",
            "Character",
            "Hero",
            "Warrior",
            "Mage",
            "Monster",
            "BossMonster",
            "MinionMonster",
            "InventoryItem",
            "Equipment",
            "Weapon"
        };

        #region Helper Methods สำหรับการตรวจสอบโครงสร้างคลาสด้วย Reflection

        /// <summary>
        /// ค้นหา Type ของคลาสตามชื่อใน namespace MidtermExam.Prob01
        /// </summary>
        private Type GetClassType(string className)
        {
            var assembly = Assembly.GetAssembly(typeof(GameEntity));
            var type = assembly.GetType($"{ExpectedNamespace}.{className}");
            Assert.IsNotNull(type, 
                $"[ไม่พบคลาส / Class Missing] ไม่พบคลาส '{className}' ใน namespace '{ExpectedNamespace}'\n" +
                $"-> กรุณาตรวจสอบว่าสร้างไฟล์ {className}.cs และประกาศ namespace MidtermExam.Prob01 ถูกต้องหรือไม่");
            return type;
        }

        /// <summary>
        /// ตรวจสอบ Field: ชื่อ, ชนิดข้อมูล (Type) และ Access Modifier (public/protected/private)
        /// </summary>
        private void AssertField(Type type, string fieldName, Type expectedFieldType, string expectedModifier)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var field = type.GetField(fieldName, bindingFlags);

            Assert.IsNotNull(field, 
                $"[{type.Name}] ไม่พบตัวแปร '{fieldName}'\n" +
                $"-> คำแนะนำ: ตรวจสอบการสะกดชื่อตัวแปร '{fieldName}' ในคลาส '{type.Name}' (Case-sensitive)");

            Assert.AreEqual(expectedFieldType, field.FieldType, 
                $"[{type.Name}.{fieldName}] ชนิดข้อมูลไม่ถูกต้อง\n" +
                $"-> คาดหวัง: {expectedFieldType.Name}, แต่พบ: {field.FieldType.Name}");

            string symbol = expectedModifier == "public" ? "+" : (expectedModifier == "protected" ? "#" : "-");
            switch (expectedModifier.ToLower())
            {
                case "public":
                    Assert.IsTrue(field.IsPublic, 
                        $"[{type.Name}.{fieldName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'public' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
                case "protected":
                    Assert.IsTrue(field.IsFamily, 
                        $"[{type.Name}.{fieldName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'protected' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
                case "private":
                    Assert.IsTrue(field.IsPrivate, 
                        $"[{type.Name}.{fieldName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'private' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
                default:
                    Assert.Fail($"Unknown modifier: {expectedModifier}");
                    break;
            }
        }

        /// <summary>
        /// ตรวจสอบ Method: ชื่อ, Return Type, Parameters, Access Modifier, virtual, override
        /// </summary>
        private void AssertMethod(Type type, string methodName, Type returnType, Type[] paramTypes, string expectedModifier, bool shouldBeVirtual, bool shouldBeOverride)
        {
            var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var methods = type.GetMethods(bindingFlags).Where(m => m.Name == methodName).ToArray();
            
            Assert.IsTrue(methods.Length > 0, 
                $"[{type.Name}] ไม่พบ Method '{methodName}'\n" +
                $"-> คำแนะนำ: ตรวจสอบว่าประกาศ Method '{methodName}' ในคลาส '{type.Name}' หรือไม่ (Case-sensitive)");

            MethodInfo targetMethod = null;
            foreach (var m in methods)
            {
                var parameters = m.GetParameters().Select(p => p.ParameterType).ToArray();
                if (parameters.SequenceEqual(paramTypes))
                {
                    targetMethod = m;
                    break;
                }
            }

            string expectedParamsStr = paramTypes.Length == 0 ? "ไม่มี parameter" : string.Join(", ", paramTypes.Select(p => p.Name));
            Assert.IsNotNull(targetMethod, 
                $"[{type.Name}.{methodName}] พารามิเตอร์ไม่ตรงตามสเปก\n" +
                $"-> สเปกที่ต้องการ: ({expectedParamsStr})");

            Assert.AreEqual(returnType, targetMethod.ReturnType, 
                $"[{type.Name}.{methodName}] ค่า Return Type ไม่ถูกต้อง\n" +
                $"-> คาดหวัง: {returnType.Name}, แต่พบ: {targetMethod.ReturnType.Name}");

            string symbol = expectedModifier == "public" ? "+" : (expectedModifier == "protected" ? "#" : "-");
            switch (expectedModifier.ToLower())
            {
                case "public":
                    Assert.IsTrue(targetMethod.IsPublic, 
                        $"[{type.Name}.{methodName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'public' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
                case "protected":
                    Assert.IsTrue(targetMethod.IsFamily, 
                        $"[{type.Name}.{methodName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'protected' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
                case "private":
                    Assert.IsTrue(targetMethod.IsPrivate, 
                        $"[{type.Name}.{methodName}] Access Modifier ไม่ถูกต้อง\n" +
                        $"-> ต้องเป็น 'private' (สัญลักษณ์ '{symbol}' ใน Diagram)");
                    break;
            }

            if (shouldBeVirtual && !shouldBeOverride)
            {
                Assert.IsTrue(targetMethod.IsVirtual && !targetMethod.IsFinal, 
                    $"[{type.Name}.{methodName}] เมธอดนี้ใน Base Class ต้องระบุคีย์เวิร์ด 'virtual'");
                Assert.AreEqual(targetMethod, targetMethod.GetBaseDefinition(), 
                    $"[{type.Name}.{methodName}] เป็นการประกาศครั้งแรก ต้องใช้ 'virtual' ไม่ใช่ 'override'");
            }

            if (shouldBeOverride)
            {
                Assert.IsTrue(targetMethod.IsVirtual, 
                    $"[{type.Name}.{methodName}] ต้องเป็นเมธอดที่ override ได้");
                Assert.AreNotEqual(targetMethod, targetMethod.GetBaseDefinition(), 
                    $"[{type.Name}.{methodName}] เมธอดนี้ใน Derived Class ต้องระบุคีย์เวิร์ด 'override' เพื่อแทนที่เมธอดของ Base Class");
            }
        }

        #endregion

        #region TC01: ตรวจสอบการมีอยู่ของคลาสทั้งหมด 11 คลาส

        /// <summary>
        /// [TC01] ตรวจสอบความครบถ้วนของคลาสทั้ง 11 คลาสใน namespace MidtermExam.Prob01
        /// 
        /// [คำสั่ง / Instruction]:
        /// สร้างคลาสให้ครบทั้ง 11 คลาสตาม Class Diagram:
        /// GameEntity, Character, Hero, Warrior, Mage, Monster, BossMonster,
        /// MinionMonster, InventoryItem, Equipment, Weapon
        /// </summary>
        [Test(Description = "TC01: ตรวจสอบว่าคลาสครบทั้ง 11 คลาสใน namespace MidtermExam.Prob01")]
        public void TC01_ClassExistence_All11ClassesExist()
        {
            var assembly = Assembly.GetAssembly(typeof(GameEntity));
            var namespaceTypes = assembly.GetTypes()
                .Where(t => t.Namespace == ExpectedNamespace)
                .Select(t => t.Name)
                .ToArray();

            foreach (var expectedClass in expectedClasses)
            {
                Assert.Contains(expectedClass, namespaceTypes, 
                    $"[TC01 ขาดคลาส] คลาส '{expectedClass}' ยังไม่ได้สร้าง หรือ namespace ไม่ใช่ '{ExpectedNamespace}'");
            }

            Assert.GreaterOrEqual(namespaceTypes.Length, expectedClasses.Length, 
                $"[TC01 จำนวนคลาสไม่ครบ] คาดหวังอย่างน้อย 11 คลาส แต่พบเพียง {namespaceTypes.Length} คลาส");
        }

        #endregion

        #region TC02: ตรวจสอบความสัมพันธ์การสืบทอด (Inheritance Hierarchy)

        /// <summary>
        /// [TC02] ตรวจสอบลำดับการสืบทอดคุณสมบัติ (Inheritance Hierarchy)
        /// 
        /// [คำสั่ง / Instruction]:
        /// กำหนด Base Class ให้ถูกต้องตามเส้นลูกศรสามเหลี่ยมกลวงใน Class Diagram:
        /// - Character สืบทอดจาก GameEntity
        /// - Hero สืบทอดจาก Character
        /// - Monster สืบทอดจาก Character
        /// - Warrior สืบทอดจาก Hero
        /// - Mage สืบทอดจาก Hero
        /// - BossMonster สืบทอดจาก Monster
        /// - MinionMonster สืบทอดจาก Monster
        /// - Equipment สืบทอดจาก InventoryItem
        /// - Weapon สืบทอดจาก Equipment
        /// </summary>
        [Test(Description = "TC02: ตรวจสอบการสืบทอดคลาส (Inheritance) ของทั้ง 11 คลาส")]
        public void TC02_Inheritance_AllRelationships()
        {
            var gameEntityType = GetClassType("GameEntity");
            var characterType = GetClassType("Character");
            var heroType = GetClassType("Hero");
            var warriorType = GetClassType("Warrior");
            var mageType = GetClassType("Mage");
            var monsterType = GetClassType("Monster");
            var bossMonsterType = GetClassType("BossMonster");
            var minionMonsterType = GetClassType("MinionMonster");
            var inventoryItemType = GetClassType("InventoryItem");
            var equipmentType = GetClassType("Equipment");
            var weaponType = GetClassType("Weapon");

            // สาย Character Hierarchy
            Assert.IsTrue(characterType.IsSubclassOf(gameEntityType), "[Inheritance] Character ต้องสืบทอดจาก (: GameEntity)");
            Assert.IsTrue(heroType.IsSubclassOf(characterType), "[Inheritance] Hero ต้องสืบทอดจาก (: Character)");
            Assert.IsTrue(monsterType.IsSubclassOf(characterType), "[Inheritance] Monster ต้องสืบทอดจาก (: Character)");
            Assert.IsTrue(warriorType.IsSubclassOf(heroType), "[Inheritance] Warrior ต้องสืบทอดจาก (: Hero)");
            Assert.IsTrue(mageType.IsSubclassOf(heroType), "[Inheritance] Mage ต้องสืบทอดจาก (: Hero)");
            Assert.IsTrue(bossMonsterType.IsSubclassOf(monsterType), "[Inheritance] BossMonster ต้องสืบทอดจาก (: Monster)");
            Assert.IsTrue(minionMonsterType.IsSubclassOf(monsterType), "[Inheritance] MinionMonster ต้องสืบทอดจาก (: Monster)");

            // สาย Item & Equipment Hierarchy
            Assert.IsTrue(equipmentType.IsSubclassOf(inventoryItemType), "[Inheritance] Equipment ต้องสืบทอดจาก (: InventoryItem)");
            Assert.IsTrue(weaponType.IsSubclassOf(equipmentType), "[Inheritance] Weapon ต้องสืบทอดจาก (: Equipment)");
        }

        #endregion

        #region TC03 - TC13: ตรวจสอบสมาชิกของแต่ละคลาส (Fields, Methods, Modifiers)

        /// <summary>
        /// [TC03] GameEntity - Base Class สูงสุดสำหรับ Entity ในเกม
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: GameEntity.cs
        /// Fields:
        ///   + string id              (public)
        ///   - Vector3 position       (private)
        ///   # int health             (protected)
        /// Methods:
        ///   + virtual void Update()
        ///   + virtual void TakeDamage(int damage)
        ///   - void Move(Vector3 direction)
        /// </summary>
        
        public string id;
        private Vector3 position;
        protected int health;
        public virtual void Update()
        {
            health = 0;
        }
        [Test(Description = "TC03: ตรวจสอบ GameEntity (Fields: id, position, health / Methods: Update, TakeDamage, Move)")]
        
        public void TC03_GameEntity_Structure()
        {
            var type = GetClassType("GameEntity");
            GameEntity gameEntity = new GameEntity();
            // ตรวจสอบ Fields
            AssertField(type, "id", typeof(string), "public");
            AssertField(type, "position", typeof(Vector3), "private");
            AssertField(type, "health", typeof(int), "protected");

            // ตรวจสอบ Methods
            AssertMethod(type, "Update", typeof(void), Type.EmptyTypes, "public", shouldBeVirtual: true, shouldBeOverride: false);
            AssertMethod(type, "TakeDamage", typeof(void), new Type[] { typeof(int) }, "public", shouldBeVirtual: true, shouldBeOverride: false);
            AssertMethod(type, "Move", typeof(void), new Type[] { typeof(Vector3) }, "private", shouldBeVirtual: false, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC04] Character - สืบทอดจาก GameEntity
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Character.cs (: GameEntity)
        /// Fields:
        ///   + string characterName   (public)
        ///   # float moveSpeed        (protected)
        ///   - int level              (private)
        /// Methods:
        ///   + virtual void Attack(GameEntity target)
        ///   # virtual void LevelUp()
        /// </summary>
        [Test(Description = "TC04: ตรวจสอบ Character (Fields: characterName, moveSpeed, level / Methods: Attack, LevelUp)")]
        public void TC04_Character_Structure()
        {
            var type = GetClassType("Character");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "characterName", typeof(string), "public");
            AssertField(type, "moveSpeed", typeof(float), "protected");
            AssertField(type, "level", typeof(int), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: false);
            AssertMethod(type, "LevelUp", typeof(void), Type.EmptyTypes, "protected", shouldBeVirtual: true, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC05] Hero - สืบทอดจาก Character
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Hero.cs (: Character)
        /// Fields:
        ///   + int currentExp         (public)
        ///   - int gold               (private)
        /// Methods:
        ///   + override void Attack(GameEntity target)
        ///   + void CollectGold(int amount)
        ///   # override void LevelUp()
        /// </summary>
        [Test(Description = "TC05: ตรวจสอบ Hero (Fields: currentExp, gold / Methods: Attack[override], CollectGold, LevelUp[override])")]
        public void TC05_Hero_Structure()
        {
            var type = GetClassType("Hero");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "currentExp", typeof(int), "public");
            AssertField(type, "gold", typeof(int), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "CollectGold", typeof(void), new Type[] { typeof(int) }, "public", shouldBeVirtual: false, shouldBeOverride: false);
            AssertMethod(type, "LevelUp", typeof(void), Type.EmptyTypes, "protected", shouldBeVirtual: true, shouldBeOverride: true);
        }

        /// <summary>
        /// [TC06] Warrior - สืบทอดจาก Hero
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Warrior.cs (: Hero)
        /// Fields:
        ///   + int shieldDefense      (public)
        ///   - float rage             (private)
        /// Methods:
        ///   + override void Attack(GameEntity target)
        ///   + void ShieldBash(GameEntity target)
        /// </summary>
        [Test(Description = "TC06: ตรวจสอบ Warrior (Fields: shieldDefense, rage / Methods: Attack[override], ShieldBash)")]
        public void TC06_Warrior_Structure()
        {
            var type = GetClassType("Warrior");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "shieldDefense", typeof(int), "public");
            AssertField(type, "rage", typeof(float), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "ShieldBash", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: false, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC07] Mage - สืบทอดจาก Hero
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Mage.cs (: Hero)
        /// Fields:
        ///   + int mana               (public)
        ///   - int spellPower         (private)
        /// Methods:
        ///   + override void Attack(GameEntity target)
        ///   + void CastSpell(GameEntity target)
        /// </summary>
        [Test(Description = "TC07: ตรวจสอบ Mage (Fields: mana, spellPower / Methods: Attack[override], CastSpell)")]
        public void TC07_Mage_Structure()
        {
            var type = GetClassType("Mage");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "mana", typeof(int), "public");
            AssertField(type, "spellPower", typeof(int), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "CastSpell", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: false, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC08] Monster - สืบทอดจาก Character
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Monster.cs (: Character)
        /// Fields:
        ///   + int baseDamage         (public)
        ///   # int aggroRange         (protected)
        /// Methods:
        ///   + override void Attack(GameEntity target)
        ///   # virtual void Roar()
        /// </summary>
        [Test(Description = "TC08: ตรวจสอบ Monster (Fields: baseDamage, aggroRange / Methods: Attack[override], Roar[virtual])")]
        public void TC08_Monster_Structure()
        {
            var type = GetClassType("Monster");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "baseDamage", typeof(int), "public");
            AssertField(type, "aggroRange", typeof(int), "protected");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "Roar", typeof(void), Type.EmptyTypes, "protected", shouldBeVirtual: true, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC09] BossMonster - สืบทอดจาก Monster
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: BossMonster.cs (: Monster)
        /// Fields:
        ///   + int phase              (public)
        ///   - bool isEnraged         (private)
        /// Methods:
        ///   + override void Attack(GameEntity target)
        ///   # override void Roar()
        ///   + void TriggerPhaseTransition()
        /// </summary>
        [Test(Description = "TC09: ตรวจสอบ BossMonster (Fields: phase, isEnraged / Methods: Attack[override], Roar[override], TriggerPhaseTransition)")]
        public void TC09_BossMonster_Structure()
        {
            var type = GetClassType("BossMonster");
            var gameEntityType = GetClassType("GameEntity");

            // ตรวจสอบ Fields
            AssertField(type, "phase", typeof(int), "public");
            AssertField(type, "isEnraged", typeof(bool), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Attack", typeof(void), new Type[] { gameEntityType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "Roar", typeof(void), Type.EmptyTypes, "protected", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "TriggerPhaseTransition", typeof(void), Type.EmptyTypes, "public", shouldBeVirtual: false, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC10] MinionMonster - สืบทอดจาก Monster
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: MinionMonster.cs (: Monster)
        /// Fields:
        ///   + int swarmBonus         (public)
        ///   - bool isAlerted         (private)
        /// Methods:
        ///   + void CallReinforcements()
        /// </summary>
        [Test(Description = "TC10: ตรวจสอบ MinionMonster (Fields: swarmBonus, isAlerted / Methods: CallReinforcements)")]
        public void TC10_MinionMonster_Structure()
        {
            var type = GetClassType("MinionMonster");

            // ตรวจสอบ Fields
            AssertField(type, "swarmBonus", typeof(int), "public");
            AssertField(type, "isAlerted", typeof(bool), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "CallReinforcements", typeof(void), Type.EmptyTypes, "public", shouldBeVirtual: false, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC11] InventoryItem - Base Class สำหรับไอเทมทั้งหมด
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: InventoryItem.cs
        /// Fields:
        ///   + string itemName        (public)
        ///   # int weight             (protected)
        ///   - int itemValue          (private)
        /// Methods:
        ///   + virtual void Use(Character user)
        /// </summary>
        [Test(Description = "TC11: ตรวจสอบ InventoryItem (Fields: itemName, weight, itemValue / Methods: Use[virtual])")]
        public void TC11_InventoryItem_Structure()
        {
            var type = GetClassType("InventoryItem");
            var characterType = GetClassType("Character");

            // ตรวจสอบ Fields
            AssertField(type, "itemName", typeof(string), "public");
            AssertField(type, "weight", typeof(int), "protected");
            AssertField(type, "itemValue", typeof(int), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Use", typeof(void), new Type[] { characterType }, "public", shouldBeVirtual: true, shouldBeOverride: false);
        }

        /// <summary>
        /// [TC12] Equipment - สืบทอดจาก InventoryItem
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Equipment.cs (: InventoryItem)
        /// Fields:
        ///   + int durability         (public)
        ///   # bool isEquipped        (protected)
        /// Methods:
        ///   + virtual void Equip(Hero hero)
        ///   + override void Use(Character user)
        /// </summary>
        [Test(Description = "TC12: ตรวจสอบ Equipment (Fields: durability, isEquipped / Methods: Equip[virtual], Use[override])")]
        public void TC12_Equipment_Structure()
        {
            var type = GetClassType("Equipment");
            var heroType = GetClassType("Hero");
            var characterType = GetClassType("Character");

            // ตรวจสอบ Fields
            AssertField(type, "durability", typeof(int), "public");
            AssertField(type, "isEquipped", typeof(bool), "protected");

            // ตรวจสอบ Methods
            AssertMethod(type, "Equip", typeof(void), new Type[] { heroType }, "public", shouldBeVirtual: true, shouldBeOverride: false);
            AssertMethod(type, "Use", typeof(void), new Type[] { characterType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
        }

        /// <summary>
        /// [TC13] Weapon - สืบทอดจาก Equipment
        /// 
        /// [คำสั่ง / Instruction]:
        /// ไฟล์: Weapon.cs (: Equipment)
        /// Fields:
        ///   + int extraDamage        (public)
        ///   - float criticalChance   (private)
        /// Methods:
        ///   + override void Equip(Hero hero)
        ///   + void Polish()
        /// </summary>
        [Test(Description = "TC13: ตรวจสอบ Weapon (Fields: extraDamage, criticalChance / Methods: Equip[override], Polish)")]
        public void TC13_Weapon_Structure()
        {
            var type = GetClassType("Weapon");
            var heroType = GetClassType("Hero");

            // ตรวจสอบ Fields
            AssertField(type, "extraDamage", typeof(int), "public");
            AssertField(type, "criticalChance", typeof(float), "private");

            // ตรวจสอบ Methods
            AssertMethod(type, "Equip", typeof(void), new Type[] { heroType }, "public", shouldBeVirtual: true, shouldBeOverride: true);
            AssertMethod(type, "Polish", typeof(void), Type.EmptyTypes, "public", shouldBeVirtual: false, shouldBeOverride: false);
        }

        #endregion
    }
}
