
public enum E_InputButton
{
    Left = 0,
    Right,
    Middle
}

public enum E_ObjectType
{
    None = -1,
    Erase,

    Player,
    Enemy,
    Coin,
    Wall,
    SafetyZone,
    GravityZone,
    IceZone,

    Max
}

public enum E_EnemyType
{
    None = -1,

    Linear,
    LinearRepeat,
    Circular,

    Max
}

public enum E_TileIndexType
{
    None = -1,

    Odd, // Ȧ�� ĭ
    Even, // ¦�� ĭ
}
public enum E_TileType
{
    Error = -2,
    None = -1,

    Wall, // ��
    SafetyZone, // ��������
    GravityZone, // �߷±���
    IceZone, // ��������
}