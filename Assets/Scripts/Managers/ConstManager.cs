//控制car的驱动模式。前驱、后驱、四驱
public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }
//轮子的前后。前轮、后轮
public enum AxelType { Front, Back }
//boss的武器。火焰武器、大锤
public enum BossWeaponType { Flamethrower, Hummer }
//Melee的攻击方式。近战、冲锋
public enum AttackType_Melee { Close, Charge }
//Melee的种类
public enum EnemyMelee_Type { Regular, Shield, Dodge, AxeThrow }
//这个是cover的等级，代表enemy的切换cover的智能度
public enum CoverPerk { Unavalible, CanTakeCover, CanTakeAndChangeCover }
//这个用来控制advance的两种状态
public enum UnstoppablePerk { Unavalible, Unstoppable }
//这个是开启扔手雷的能力
public enum GrenadePerk { Unavalible, CanThrowGrenade }
//Range的武器model的持有方式
public enum Enemy_RangeWeaponHoldType { Common, LowHold, HighHold };
//Melee的武器类型
public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
//range的武器类型
public enum Enemy_RangeWeaponType { Pistol, Revolver, Shotgun, AutoRifle, Rifle, Random }
//Enemy的种类
public enum EnemyType { Melee, Range, Boss, Random }
//弹药箱的种类
public enum AmmoBoxType { smallBox, bigBox }
//地图的snappoint的类型
public enum SnapPointType
{
    Enter,
    Exit
}
//player后背武器的持有方式
public enum HangType { LowBackHang, BackHang, SideHang }
//player的武器种类
public enum WeaponType
{
    Pistol,
    Revolver,
    AutoRifle,
    Shotgun,
    Rifle
}
//player的射击方式
public enum ShootType
{
    Single,
    Auto
}
//player装备武器的动画种类
public enum EquipType { SideEquipAnimation, BackEquipAnimation };
//player手拿武器的几种hold方式
public enum HoldType { CommonHold = 1, LowHold, HighHold };