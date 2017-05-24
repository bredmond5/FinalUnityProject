using UnityEngine;

/*
 This class handles switching the guns that the player has equipped. It also handles keeping track
 of the ammo and turning off the player's ability to use the gun when needed.
 */ 

public class PlayerGunsManager : MonoBehaviour
{
	[HideInInspector] public GameObject m_Player;
	[HideInInspector] public GameObject m_AmmoManager;
	public GameObject[] m_Guns; 
	public Animator m_PlayerRifleController;
	public Animator m_PlayerPistolController;
	public Animator m_PlayerSMGController;
	public Animator m_PlayerPistolControllerSemiAuto;
	public Animator m_PlayerPunchController;
    public Animator m_PlayerSwordController;

    private int m_CurWeapon; //Reference to which gun the player is using in the array.
	private Transform m_WeaponMount;
	private GameObject m_CurWeaponSelected; //The gun in the scene.
	private int [,] m_AmmoInGuns; //Keeps track of how much ammo is in each gun. 
	private int[,] m_AmmoInGunsBeforeFight; //Holds the ammo in each gun before the fight starts. 
	private GameObject[] m_GunsBeforeFight;
	private GameObject m_Arms;

	void Awake ()
	{
        if (!m_PlayerRifleController) m_PlayerRifleController = GameObject.Find("PlayerRifleController").GetComponent<Animator>();
        if (!m_PlayerPistolController) m_PlayerPistolController = GameObject.Find("PlayerPistolController").GetComponent<Animator>();
        if (!m_PlayerSMGController) m_PlayerSMGController = GameObject.Find("PlayerSMGController").GetComponent<Animator>();
        if (!m_PlayerPistolControllerSemiAuto) m_PlayerPistolControllerSemiAuto = GameObject.Find("PlayerPistolControllerSemiAuto").GetComponent<Animator>();
        if (!m_PlayerPunchController) m_PlayerPunchController = GameObject.Find("PlayerPunchController").GetComponent<Animator>();
        if (!m_PlayerSwordController) m_PlayerSwordController = GameObject.Find("PlayerSwordController").GetComponent<Animator>();


        m_CurWeapon = 0;
		m_WeaponMount = findInChildren(m_Player, "weapon_mount").transform;
		m_Arms = findInChildren (m_Player, "Arms");

		if (m_Guns.Length != 0) {
			getAmmos ();
			makeTheWeapon ();
		}
	}

	private void getAmmos()
	{
		m_AmmoInGuns = new int[m_Guns.Length, 2];
		for (int i = 0; i < m_Guns.Length; i++) {
			PlayerShooting playerShooting = m_Guns [i].GetComponentInChildren<PlayerShooting> ();
			if (playerShooting != null) {
				m_AmmoInGuns [i, 0] = playerShooting.m_AmmoPerClip;
				m_AmmoInGuns [i, 1] = playerShooting.m_Ammo - playerShooting.m_AmmoPerClip;
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.H) && m_Guns.Length != 0) {
			switchWeapon (false);
		} else if (Input.GetKeyDown (KeyCode.J) && m_Guns.Length != 0) {
			switchWeapon (true);
		}
	}

	private void switchWeapon(bool forwards)
	{
		m_AmmoManager.SetActive (true);

		m_AmmoInGuns [m_CurWeapon, 0] = AmmoManager.m_AmmoInClip;
		m_AmmoInGuns [m_CurWeapon, 1] = AmmoManager.m_AmmoLeft;

		if (forwards) {
			if (m_CurWeapon + 1 < m_Guns.Length)
				m_CurWeapon++;
			else
				m_CurWeapon = 0;
		} else {
			if (m_CurWeapon > 0)
				m_CurWeapon--;
			else
				m_CurWeapon = m_Guns.Length - 1;
		}

		Destroy (m_CurWeaponSelected);

		makeTheWeapon ();
	}

	private void makeTheWeapon()
	{
		instantiateWeapon ();
		PlayerShooting playerShooting = m_CurWeaponSelected.GetComponentInChildren<PlayerShooting> ();

		if (playerShooting != null) {
			finishGun (playerShooting);
		} else {
			finishMeleeWeapon();
		}
	}

	private void instantiateWeapon()
	{
		Quaternion rotationWanted = m_Guns[m_CurWeapon].transform.localRotation;
		Vector3 positionWanted = m_Guns[m_CurWeapon].transform.localPosition;

		m_CurWeaponSelected = Instantiate(m_Guns[m_CurWeapon])as GameObject;
		m_CurWeaponSelected.transform.parent = m_WeaponMount;
		m_CurWeaponSelected.transform.localRotation = rotationWanted;
		m_CurWeaponSelected.transform.localPosition = positionWanted;
	}

	void finishGun(PlayerShooting playerShooting)
	{
		AmmoManager.m_AmmoInClip = m_AmmoInGuns [m_CurWeapon, 0];
		AmmoManager.m_AmmoLeft = m_AmmoInGuns [m_CurWeapon, 1];

		playerShooting.m_FirstPersonController = m_Player.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		playerShooting.setUpReferences ();

		putOnCorrectGunAnimator (playerShooting);
		playerShooting.checkAnimators ();

		if (AmmoManager.m_AmmoInClip == 0) {
			playerShooting.m_Reload.noBullets = true;
			Debug.Log ("Turning canShoot off");
			playerShooting.m_CanShoot = false;
		} else {
			playerShooting.m_CanShoot = true;
		}

		m_CurWeaponSelected.SetActive (true);
	}

	private void finishMeleeWeapon()
	{
		m_AmmoManager.SetActive (false);

		PlayerMelee playerMelee = m_CurWeaponSelected.GetComponentInChildren<PlayerMelee> ();
		playerMelee.m_FirstPersonController = m_Player.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();
		playerMelee.setUpReferences ();

		putOnCorrectMeleeAnimator (playerMelee);

		m_CurWeaponSelected.SetActive (true);

	}

	private void putOnCorrectMeleeAnimator(PlayerMelee playerMelee)
	{
		string animatorWanted = playerMelee.returnAnimatorName();
		Debug.Log (animatorWanted);
		Animator animator = m_Arms.GetComponentInChildren<Animator> ();

		if (animatorWanted == "PlayerPunchController") {
			animator.runtimeAnimatorController = m_PlayerPunchController.runtimeAnimatorController;
		}else if(animatorWanted == "PlayerSwordController")
        {
            animator.runtimeAnimatorController = m_PlayerSwordController.runtimeAnimatorController;
        }

        Debug.Log("Putting the animator on!");
        playerMelee.m_MeleeAnim = animator;
    }

    private void putOnCorrectGunAnimator(PlayerShooting playerShooting)
	{
		string animatorWanted = playerShooting.returnAnimatorName();
		Debug.Log (animatorWanted);
		Animator animator = m_Arms.GetComponentInChildren<Animator> ();

		if (animatorWanted == "RifleController") {
			animator.runtimeAnimatorController = m_PlayerRifleController.runtimeAnimatorController;

		} else if (animatorWanted == "PistolController") {
			animator.runtimeAnimatorController = m_PlayerPistolController.runtimeAnimatorController;

		} else if (animatorWanted == "SMGController") {
			animator.runtimeAnimatorController = m_PlayerSMGController.runtimeAnimatorController;

		}else if(animatorWanted == "PistolControllerSemiAuto"){
			animator.runtimeAnimatorController = m_PlayerPistolControllerSemiAuto.runtimeAnimatorController;

		} else {
			Debug.Log ("Please put a gunanimatorcontoller on the gun.");
		}
		playerShooting.m_ArmAnimator = animator;
	}

	public void turnOffShootingAbility()
	{
		PlayerShooting playerShooting = m_CurWeaponSelected.GetComponentInChildren<PlayerShooting> ();
		if (playerShooting != null) {
			playerShooting.m_CanShoot = false;
		} else {
			PlayerMelee playerMelee =  m_CurWeaponSelected.GetComponentInChildren<PlayerMelee> ();
			playerMelee.m_CanAttack = false;
		}
	}

	public void turnOnShootingAbility()
	{
		PlayerShooting playerShooting = m_CurWeaponSelected.GetComponentInChildren<PlayerShooting> ();
		if (playerShooting != null) {
			playerShooting.m_CanShoot = true;
		} else {
			m_CurWeaponSelected.GetComponentInParent<PlayerMelee> ().m_CanAttack = true;
		}
	}

	public void turnOffGunAndArms()
	{
		m_Arms.SetActive(false);
	}

	public void turnOnGunAndArms()
	{
		m_Arms.SetActive(true);
	}

	public void getAmmosAndGunsBeforeFight()
	{
		m_AmmoInGunsBeforeFight = m_AmmoInGuns;
		m_GunsBeforeFight = m_Guns;
	}

	public void restoreAmmosAndGuns()
	{
		m_AmmoInGuns = m_AmmoInGunsBeforeFight;
		m_Guns = m_GunsBeforeFight;
	}

	public void addGun(GameObject gun, int whichGunToSwitchFor)
	{
		if (whichGunToSwitchFor < 0) {
			addNewGunSpot (gun);

		} else {
			switchGunOut (gun, whichGunToSwitchFor);
		}

		gun.SetActive (false);
		Debug.Log ("You found a new weapon!");
	}

	private void addNewGunSpot(GameObject gun)
	{
		GameObject[] newGunsArray = new GameObject[m_Guns.Length + 1];
		newGunsArray [newGunsArray.Length - 1] = gun;

		for (int i = 0; i < m_Guns.Length; i++) {
			newGunsArray [i] = m_Guns [i];
		}
		m_Guns = newGunsArray;
		makeGunUsable (m_Guns [m_Guns.Length - 1]);
		changeAmmoArrays ();
		m_CurWeapon = newGunsArray.Length - 2;
		switchWeapon (true);
	}

	private void switchGunOut(GameObject gun, int whichGunToSwitchFor)
	{
		m_Guns [whichGunToSwitchFor] = gun;
		makeGunUsable (m_Guns [whichGunToSwitchFor]);
		m_CurWeapon = whichGunToSwitchFor - 1;
		switchWeapon (true);

		PlayerShooting playerShooting = m_Guns [whichGunToSwitchFor].GetComponentInChildren<PlayerShooting> ();

		m_AmmoInGuns[whichGunToSwitchFor, 0] = playerShooting.m_AmmoPerClip;
		m_AmmoInGuns[whichGunToSwitchFor, 1] = playerShooting.m_Ammo - playerShooting.m_AmmoPerClip;

		AmmoManager.m_AmmoInClip = m_AmmoInGuns [m_CurWeapon, 0];
		AmmoManager.m_AmmoLeft = m_AmmoInGuns [m_CurWeapon, 1];
	}

	private void makeGunUsable(GameObject gun)
	{
		gun.transform.rotation = new Quaternion (0, 0, 0, 0);

		gun.GetComponentInChildren<BoxCollider>().enabled = false;
		gun.GetComponentInChildren<SphereCollider> ().enabled = false;
		gun.GetComponentInChildren<GunOnTriggerEnter> ().enabled = false;
		gun.GetComponentInChildren<Reload> ().enabled = true;
		gun.GetComponentInChildren<PlayerShooting> ().enabled = true;
		gun.GetComponentInChildren<AudioSource> ().enabled = true;
	}

	private void changeAmmoArrays()
	{
		int[,] newAmmoArray = new int [m_Guns.Length, 2];
		for (int i = 0; i < (m_AmmoInGuns.Length / 2); i++) {
			for (int j = 0; j < 2; j++) {
				newAmmoArray [i,j] = m_AmmoInGuns [i,j];
			}
		}
		PlayerShooting playerShooting = m_Guns [m_Guns.Length - 1].GetComponentInChildren<PlayerShooting> ();

		newAmmoArray [(newAmmoArray.Length / 2) - 1, 0] = playerShooting.m_AmmoPerClip;
		newAmmoArray [(newAmmoArray.Length / 2) - 1, 1] = playerShooting.m_Ammo - playerShooting.m_AmmoPerClip;

		m_AmmoInGuns = newAmmoArray;
	}

	public GameObject findInChildren(GameObject go, string name)
	{
		foreach (Transform x in go.GetComponentsInChildren<Transform>()) {
			if (x.gameObject.name == name)
				return x.gameObject;
		}
		return null;
	}
}

