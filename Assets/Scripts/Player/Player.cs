using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;

    public PlayerControls controls { get; private set; }
    public Player_AimController aim { get; private set; }
    public Player_Movement movement { get; private set; }
    public Player_WeaponController weapon { get; private set; }
    public Player_WeaponVisuals weaponVisuals { get; private set; }
    public Player_Interaction interaction { get; private set; }
    public Player_Health health { get; private set; }
    public Ragdoll ragdoll { get; private set; }

    public Animator anim { get; private set; }

    public bool controlsEnabled { get; private set; }

    private void Awake()
    {
        aim = GetComponent<Player_AimController>();
        movement = GetComponent<Player_Movement>();
        weapon = GetComponent<Player_WeaponController>();
        weaponVisuals = GetComponent<Player_WeaponVisuals>();
        interaction = GetComponent<Player_Interaction>();
        health = GetComponent<Player_Health>();
        ragdoll = GetComponent<Ragdoll>();
        anim = GetComponentInChildren<Animator>();
        controls = ControlsManager.instance.controls;
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Character.UI_MissionToolTipSwitch.performed += ctx => UI.instance.inGameUI.SwitchMissionToolTip();
        controls.Character.UI_Pause.performed += ctx => UI.instance.PauseSwitch();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    public void SetControlEnabledTo(bool enabled) => controlsEnabled = enabled;
}
