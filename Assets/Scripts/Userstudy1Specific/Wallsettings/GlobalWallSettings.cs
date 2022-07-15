using UnityEngine;

public class GlobalWallSettings : MonoBehaviour
{
    [Header("Solid Wall Settings")]
    public bool solid_useStaticValues = false;

    public int solid_staticCurrent1 = 15;
    public int solid_staticCurrent2 = 15;
    public int solid_staticWidth1 = 100;
    public int solid_staticWidth2 = 100;

    [Header("Shock Settings")]
    public bool shock_useStaticValues = false;

    public int shock_staticCurrent1 = 15;
    public int shock_staticCurrentBB1 = 15;
    public int shock_staticCurrentBB2 = 15;
    public int shock_staticWidth1 = 100;
    public int shock_staticWidth2 = 100;
    public int shock_staticMaxWidth1 = 100;
    public int shock_staticMaxWidth2 = 100;

    private void Awake()
    {
        SolidWallSettings.useStaticValues = solid_useStaticValues;
        SolidWallSettings.staticCurrent1 = solid_staticCurrent1;
        SolidWallSettings.staticCurrent2 = solid_staticCurrent2;
        SolidWallSettings.staticWidth1 = solid_staticWidth1;
        SolidWallSettings.staticWidth2 = solid_staticWidth2;

        MagnetWallSettings.useStaticValues = shock_useStaticValues;
        MagnetWallSettings.staticCurrent1 = shock_staticCurrent1;
        MagnetWallSettings.staticWidth1 = shock_staticWidth1;
        MagnetWallSettings.staticWidth2 = shock_staticWidth2;
        MagnetWallSettings.staticCurrentBB1 = shock_staticCurrentBB1;
        MagnetWallSettings.staticCurrentBB2 = shock_staticCurrentBB2;
        MagnetWallSettings.staticMaxWidth1 = shock_staticMaxWidth1;
        MagnetWallSettings.staticMaxWidth2 = shock_staticMaxWidth2;
    }

    public void Start()
    {
        ApplyValues();
    }

    public void ApplyValues()
    {
        if (!Application.isPlaying)
            return;

        string msg = "Setting EMS Values: \n"
            + "Solid: " + solid_staticCurrent1 + " mA | " + solid_staticWidth1 + " Hz"
            + ", " + solid_staticCurrent2 + " mA | " + solid_staticWidth2 + " Hz \n"
            + "Shock: " + shock_staticCurrent1 + " mA | " + shock_staticWidth1 + " Hz"
            + ", " + shock_staticCurrent1 + " mA | " + shock_staticWidth2 + " Hz"
            ;

        Logger.LogLine(msg);
    }
}