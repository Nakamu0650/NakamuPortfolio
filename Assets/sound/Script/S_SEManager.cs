using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;

public class S_SEManager : MonoBehaviour
{
    public static S_SoundManager S_Sound;
    public static Transform S_Lisener;
    public static Transform S_CameraTransrom;
    public static CriWare.Assets.CriAtomCueReference menunoise;
    public static CriWare.Assets.CriAtomCueReference gamestart;
    public static CriWare.Assets.CriAtomCueReference menuOpen;
    public static CriWare.Assets.CriAtomCueReference menuClose;
    public static CriWare.Assets.CriAtomCueReference StartselectSE;
    public static CriWare.Assets.CriAtomCueReference notStartSelectedSE;
    public static CriWare.Assets.CriAtomCueReference selectSE;
    public static CriWare.Assets.CriAtomCueReference cannotUseWreath;
    public static CriWare.Assets.CriAtomCueReference textSE;
    public static CriWare.Assets.CriAtomCueReference targetCameraOnSE;
    public static CriWare.Assets.CriAtomCueReference targetCameraOffSE;
    public static CriWare.Assets.CriAtomCueReference playerDamegeSE;
    public static CriWare.Assets.CriAtomCueReference playerWalkSE;
    public static CriWare.Assets.CriAtomCueReference playerRunSE;
    public static CriWare.Assets.CriAtomCueReference playerBrakeSE;
    public static CriWare.Assets.CriAtomCueReference playerLandingSE;
    public static CriWare.Assets.CriAtomCueReference playerJumpSE;
    public static CriWare.Assets.CriAtomCueReference playerFlySE;
    public static CriWare.Assets.CriAtomCueReference playerUmbrellaTakeOutSE;
    public static CriWare.Assets.CriAtomCueReference playerUmbrellaPutAwaySE;
    public static CriWare.Assets.CriAtomCueReference playerAttackPrepareSE;
    public static CriWare.Assets.CriAtomCueReference playerUmbrellaAttackHiSE;
    public static CriWare.Assets.CriAtomCueReference playerAvoidSE;
    public static CriWare.Assets.CriAtomCueReference playerAttackSE;
    public static CriWare.Assets.CriAtomCueReference playerEquipSE1;
    public static CriWare.Assets.CriAtomCueReference playerEquipSE2;
    public static CriWare.Assets.CriAtomCueReference flowersBloomSE;
    public static CriWare.Assets.CriAtomCueReference flowersBloomSE2;
    public static CriWare.Assets.CriAtomCueReference lackOfEnergySE;
    public static CriWare.Assets.CriAtomCueReference roseAttackSE;
    public static CriWare.Assets.CriAtomCueReference RoseAttackChargeSE;
    public static CriWare.Assets.CriAtomCueReference roseAttackhitSE;
    public static CriWare.Assets.CriAtomCueReference roseRotationAttackSE;
    public static CriWare.Assets.CriAtomCueReference roseRotationAttackStopSE;
    public static CriWare.Assets.CriAtomCueReference sunflowerLaserChargeSE;
    public static CriWare.Assets.CriAtomCueReference sunflowerLaserShot1SE;
    public static CriWare.Assets.CriAtomCueReference sunflowerLaserShot2SE;
    public static CriWare.Assets.CriAtomCueReference sunflowerLaserShot3SE;
    public static CriWare.Assets.CriAtomCueReference sunflowerLaserHit;
    public static CriWare.Assets.CriAtomCueReference balsamChargeSE;
    public static CriWare.Assets.CriAtomCueReference balsamDistanceSE;
    public static CriWare.Assets.CriAtomCueReference balsamHitSE;
    public static CriWare.Assets.CriAtomCueReference balsamHitLandingSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosisSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosisGentianSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosisCherryBlossomsSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosishibiscusSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosisNemophilaSE;
    public static CriWare.Assets.CriAtomCueReference playerMetamorphosisPoisionSE;   
    public static CriWare.Assets.CriAtomCueReference playerPoisionAttackSE1;
    public static CriWare.Assets.CriAtomCueReference playerPoisionAttackSE2;
    public static CriWare.Assets.CriAtomCueReference playerPoisionAttackSE3;
    public static CriWare.Assets.CriAtomCueReference playerHibiscusAttackSE;
    public static CriWare.Assets.CriAtomCueReference playerNemophilaAttackSE;
    public static CriWare.Assets.CriAtomCueReference shieldSE;
    public static CriWare.Assets.CriAtomCueReference shieldBreakeSE;
    public static CriWare.Assets.CriAtomCueReference shieldParySE;
    public static CriWare.Assets.CriAtomCueReference recoverySE;
    public static CriWare.Assets.CriAtomCueReference energyGetSE;
    public static CriWare.Assets.CriAtomCueReference acceleratorsSE;
    public static CriWare.Assets.CriAtomCueReference acceleratorsComboSE;
    public static CriWare.Assets.CriAtomCueReference treeSE1;
    public static CriWare.Assets.CriAtomCueReference treeSE2;
    public static CriWare.Assets.CriAtomCueReference boarvoiceSE;
    public static CriWare.Assets.CriAtomCueReference boarvoice2SE;
    public static CriWare.Assets.CriAtomCueReference boarwalkSE;
    public static CriWare.Assets.CriAtomCueReference crowVoiceSE;
    public static CriWare.Assets.CriAtomCueReference bearVoiceSE;
    public static CriWare.Assets.CriAtomCueReference enemyDamageSE;
    public static CriWare.Assets.CriAtomCueReference enemyDeathSE;
    public static CriWare.Assets.CriAtomCueReference lila_L_thunderSE;
    public static CriWare.Assets.CriAtomCueReference lila_bard_attackSE;
    public static CriWare.Assets.CriAtomCueReference lila_WarpSE;
    public static CriWare.Assets.CriAtomCueReference lila_WarpSE2;
    public static CriWare.Assets.CriAtomCueReference lila_explosion_poisonSE;
    public static CriWare.Assets.CriAtomCueReference lila_shockWaveSE;
    public static CriWare.Assets.CriAtomCueReference lila_BomSE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingPoisonLakeSE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingLthunderSE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingBom1SE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingBom2SE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingBardSE;
    public static CriWare.Assets.CriAtomCueReference lila_chantingShockWaveSE;
    public static CriWare.Assets.CriAtomCueReference lila_magicCircleSE;
    public static CriWare.Assets.CriAtomCueReference lila_Voice0;
    public static CriWare.Assets.CriAtomCueReference lila_Voice1;
    public static CriWare.Assets.CriAtomCueReference lila_Voice2;
    public static CriWare.Assets.CriAtomCueReference lila_Voice3;
    public static CriWare.Assets.CriAtomCueReference lila_Voice4;


    public static List<string> playerWalkAisacName = new List<string>();



    //public  static Transform S_playerTransform;
    //public static Transform S_enemyTransform;


    // Start is called before the first frame update
    void Start()
    {
        S_Sound = S_SoundManager.Instance;
        S_Sound.SetListenerTransform(S_Lisener);
        S_Sound.DisposeDummyNativeListener();
    }

    // Update is called once per frame
    void Update()
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
            
        

        
            
        
    }

    public static void PlayMenu(bool isMenuOpen)
    {
        if (isMenuOpen)
        {
            PlayMenuNoiseSE(S_Lisener);
        }
        else
        {
            S_Sound.Stop("menunoise");
        }
    }


    public static void SEStop(string key)
    {
        S_Sound.Stop(key);  
    }

    public static void PlayMenuNoiseSE(Transform playertransform)
    {
        S_Sound.PlaySound("menunoise", menunoise.AcbAsset.Handle, menunoise.CueId, playertransform, false);
    }
   public static void PlayStartgameSE(Transform playertransform)
    {
        S_Sound.PlaySound("Sraetgame", gamestart.AcbAsset.Handle, gamestart.CueId, playertransform, false);
    }
    public static void PlayMenuOpenSE(Transform playertransform)
    {
        S_Sound.PlaySound("menuopen", menuOpen.AcbAsset.Handle, menuOpen.CueId, playertransform, false);
    }

    public static void PlayMenuCloseSE(Transform playertransform)
    {
        S_Sound.PlaySound("menuclose", menuClose.AcbAsset.Handle, menuClose.CueId, playertransform, false);
    }

    public static void PlayStartSelectSE(Transform playertransform)
    {
        S_Sound.PlaySound("startselect", StartselectSE.AcbAsset.Handle, StartselectSE.CueId, playertransform, false);
    }

    public static void PlayCannotUseWreathSE(Transform playertransform)
    {
        S_Sound.PlaySound("cannotusewreth", cannotUseWreath.AcbAsset.Handle, cannotUseWreath.CueId, playertransform, false);
    }

    public static void PlayNotStartSelectSE(Transform playertransform)
    {
        S_Sound.PlaySound("notstartselect", notStartSelectedSE.AcbAsset.Handle, notStartSelectedSE.CueId, playertransform, false);
    }

    public static void PlaySelectSE(Transform playertransform)
    {
        S_Sound.PlaySound("select", selectSE.AcbAsset.Handle, selectSE.CueId, playertransform, false);
    }

    public static void PlayTextSE(Transform playertransform)
    {
        S_Sound.PlaySound("text", textSE.AcbAsset.Handle, textSE.CueId, playertransform, false);
    }

    public static void PlayTargetCameraOnSE(Transform playertransform)
    {
        S_Sound.PlaySound("targetcameraOn", targetCameraOnSE.AcbAsset.Handle, targetCameraOnSE.CueId, playertransform, false);
    }

    public static void PlayTargetCameraOffSE(Transform playertransform)
    {
        S_Sound.PlaySound("targetcameraOff", targetCameraOffSE.AcbAsset.Handle, targetCameraOffSE.CueId, playertransform, false);
    }

    public static void PlayPlayerDamageSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerDamage", playerDamegeSE.AcbAsset.Handle, playerDamegeSE.CueId, playertransform, false);
    }
    public static void PlayPlayerWalkSE(Transform playertransform)
    {
        S_Sound.PlaySoundAisac("playerWalk", playerWalkSE.AcbAsset.Handle, playerWalkSE.CueId, playerWalkAisacName, playertransform, false);
    }

    public static void PlayPlayerRunSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerRun", playerRunSE.AcbAsset.Handle, playerRunSE.CueId, playertransform, false);
    }
    public static void PlayPlayerBrakeSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerRunbrake", playerBrakeSE.AcbAsset.Handle, playerBrakeSE.CueId, playertransform, false);
    }

    public static void PlayPlayerLandingSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerLanding", playerLandingSE.AcbAsset.Handle, playerLandingSE.CueId, playertransform, false);
    }
    public static void PlayPlayerJumpSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerjump", playerJumpSE.AcbAsset.Handle, playerJumpSE.CueId, playertransform, false);
    }


    public static void PlayPlayerFlySE(Transform playertransform)
    {
        S_Sound.PlaySound("playerfly", playerFlySE.AcbAsset.Handle, playerFlySE.CueId, playertransform, false);
    }

    public static void PlayPlayerUmbrellaTakeOutSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerumbrellaTakeout", playerUmbrellaTakeOutSE.AcbAsset.Handle, playerUmbrellaTakeOutSE.CueId, playertransform, false);
    }

    public static void PlayPlayerUmbrellaPutAwaySE(Transform playertransform)
    {
        S_Sound.PlaySound("playerumbrellaPutAway", playerUmbrellaPutAwaySE.AcbAsset.Handle, playerUmbrellaPutAwaySE.CueId, playertransform, false);
    }

    public static void PlayPlayerUmbrellaAttackHitSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerumbrellaAttackHit", playerUmbrellaAttackHiSE.AcbAsset.Handle, playerUmbrellaAttackHiSE.CueId, playertransform, false);
    }

    public static void PlayPlayerAttackPrepareSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerattackprepare", playerAttackPrepareSE.AcbAsset.Handle, playerAttackPrepareSE.CueId, playertransform, false);
    }

    public static void PlayPlayerAvoidSE(Transform playertransform)
    {
        S_Sound.PlaySound("playeravoid", playerAvoidSE.AcbAsset.Handle, playerAvoidSE.CueId, playertransform, false);
    }
    public static void PlayPlayerAttackSE(Transform playertransform)
    {
        S_Sound.PlaySound("playerattack", playerAttackSE.AcbAsset.Handle, playerAttackSE.CueId, playertransform, false);
    }

    public static void PlayPlayerEquipSE1(Transform playertransform)
    {
        S_Sound.PlaySound("playerEquip", playerEquipSE1.AcbAsset.Handle, playerEquipSE1.CueId, playertransform, false);
    }
    public static void PlayPlayerEquipSE2(Transform playertransform)
    {
        S_Sound.PlaySound("playerEquip2", playerEquipSE2.AcbAsset.Handle, playerEquipSE2.CueId, playertransform, false);
    }

    public static void PlayFlowersBloomSE(Transform playertransform)
    {
        S_Sound.PlaySound("flowerBloom", flowersBloomSE.AcbAsset.Handle, flowersBloomSE.CueId, playertransform, false);
    }
    public static void PlayFlowersBloomSE2(Transform playertransform)
    {
        S_Sound.PlaySound("flowerBloom2", flowersBloomSE2.AcbAsset.Handle, flowersBloomSE2.CueId, playertransform, false);
    }

    public static void PlaylackOfEnegy2(Transform playertransform)
    {
        S_Sound.PlaySound("lackOfEnery", lackOfEnergySE.AcbAsset.Handle, lackOfEnergySE.CueId, playertransform, false);
    }

    public static void PlayGetEnergy(Transform playertransform)
    {
        S_Sound.PlaySound("getEnergy", energyGetSE.AcbAsset.Handle, energyGetSE.CueId, playertransform, false);
    }

    public static void PlayRoseAttackSE(Transform playertransform)
    {
        S_Sound.PlaySound("roseattack", roseAttackSE.AcbAsset.Handle, roseAttackSE.CueId, playertransform, false);
    }

    public static void PlayRoseAttackChargeSE(Transform playertransform)
    {
        S_Sound.PlaySound("roseattackCharge", RoseAttackChargeSE.AcbAsset.Handle, RoseAttackChargeSE.CueId, playertransform, false);
    }

    public static void PlayRoseAttackHitSE(Transform playertransform)
    {
        S_Sound.PlaySound("roseattackhit", roseAttackhitSE.AcbAsset.Handle, roseAttackhitSE.CueId, playertransform, false);
    }

    public static void PlayRoseRotationAttackSE(Transform playertransform)
    {
        S_Sound.PlaySound("roserotationattack", roseRotationAttackSE.AcbAsset.Handle, roseRotationAttackSE.CueId, playertransform, false);
    }

    public static void PlayRoseRotationAttackStopSE(Transform playertransform)
    {
        S_Sound.PlaySound("roserotationattackstop", roseRotationAttackStopSE.AcbAsset.Handle, roseRotationAttackStopSE.CueId, playertransform, false);
    }

    public static void PlaySunflowerLaserChargeSE(Transform playertransform)
    {
        S_Sound.PlaySound("sunflowerLaserCharge", sunflowerLaserChargeSE.AcbAsset.Handle, sunflowerLaserChargeSE.CueId, playertransform, false);
    }

    

    public static void PlaySunflowerLaserShot1SE(Transform playertransform)
    {
        S_Sound.PlaySound("sunflowerLaserShot1", sunflowerLaserShot1SE.AcbAsset.Handle, sunflowerLaserShot1SE.CueId, playertransform, false);
    }
    public static void PlaySunflowerLaserShot2SE(Transform playertransform)
    {
        S_Sound.PlaySound("sunflowerLaserShot2", sunflowerLaserShot2SE.AcbAsset.Handle, sunflowerLaserShot2SE.CueId, playertransform, false);
    }
    public static void PlaySunflowerLaserShot3SE(Transform playertransform)
    {
        S_Sound.PlaySound("sunflowerLaserShot3", sunflowerLaserShot3SE.AcbAsset.Handle, sunflowerLaserShot3SE.CueId, playertransform, false);
    }

    public static void PlaySunflowerLaserHitSE(Transform playertransform)
    {
        S_Sound.PlaySound("sunflowerLaserHit", sunflowerLaserHit.AcbAsset.Handle, sunflowerLaserHit.CueId, playertransform, false);
    }

    public static void PlayBalsamChargeSE(Transform playertransform)
    {
        //S_Sound.PlaySound("balsamCharge", balsamChargeSE.AcbAsset.Handle, balsamChargeSE.CueId, playertransform, false);
    }
    public static void PlayBalsamDistanceSE(Transform playertransform)
    {
        S_Sound.PlaySound("balsamDistance", balsamDistanceSE.AcbAsset.Handle, balsamDistanceSE.CueId, playertransform, false);
    }
    public static void PlayBalsamHitSE(Transform playertransform)
    {
        S_Sound.PlaySound("balsamHit", balsamHitSE.AcbAsset.Handle, balsamHitSE.CueId, playertransform, false);
    }
    public static void PlayBalsamHitLandingSE(Transform playertransform)
    {
        S_Sound.PlaySound("balsamHitLnading", balsamHitLandingSE.AcbAsset.Handle, balsamHitLandingSE.CueId, playertransform, true);
    }


    public static void PlayShieldSE(Transform playertransform)
    {
        S_Sound.PlaySound("shield", shieldSE.AcbAsset.Handle, shieldSE.CueId, playertransform, false);
    }

    public static void PlayShieldBreakeSE(Transform playertransform)
    {
        S_Sound.PlaySound("shieldbreake", shieldBreakeSE.AcbAsset.Handle, shieldBreakeSE.CueId, playertransform, false);
    }

    public static void PlayShieldParySE(Transform playertransform)
    {
        S_Sound.PlaySound("shieldpary", shieldParySE.AcbAsset.Handle, shieldParySE.CueId, playertransform, false);
    }

    public static void PlayRecoverySE(Transform playertransform)
    {
        S_Sound.PlaySound("Recovery", recoverySE.AcbAsset.Handle, recoverySE.CueId, playertransform, false);
    }

    public static void PlayPlayerMetamorphosisSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosis", playerMetamorphosisSE.AcbAsset.Handle, playerMetamorphosisSE.CueId, playertransform, false);
    }
    public static void PlayPlayerMetamorphosisGentianSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosisGentian", playerMetamorphosisGentianSE.AcbAsset.Handle, playerMetamorphosisGentianSE.CueId, playertransform, false);
    }
    public static void PlayPlayerMetamorphosisCherryBlossamSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosisCherry",playerMetamorphosisCherryBlossomsSE.AcbAsset.Handle, playerMetamorphosisCherryBlossomsSE.CueId, playertransform, false);
    }
    public static void PlayPlayerMetamorphosisHibiscusSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosisHibiscus", playerMetamorphosishibiscusSE.AcbAsset.Handle, playerMetamorphosishibiscusSE.CueId, playertransform, false);
    }
    public static void PlayPlayerMetamorphosisNemophilaSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosisNemophila", playerMetamorphosisNemophilaSE.AcbAsset.Handle, playerMetamorphosisNemophilaSE.CueId, playertransform, false);
    }
    public static void PlayPlayerMetamorphosisPoisionSE(Transform playertransform)
    {
        S_Sound.PlaySound("metmorphosisPoision", playerMetamorphosisPoisionSE.AcbAsset.Handle, playerMetamorphosisPoisionSE.CueId, playertransform, false);
    }

   

    public static void PlayPlayerPoisionAttackSE1(Transform playertransform)
    {
        S_Sound.PlaySound("PoisionAttack1", playerPoisionAttackSE1.AcbAsset.Handle, playerPoisionAttackSE1.CueId, playertransform, false);
    }

    public static void PlayPlayerPoisionAttackSE2(Transform playertransform)
    {
        S_Sound.PlaySound("PoisionAttack2", playerPoisionAttackSE2.AcbAsset.Handle, playerPoisionAttackSE2.CueId, playertransform, false);
    }

    public static void PlayPlayerPoisionAttackSE3(Transform playertransform)
    {
        S_Sound.PlaySound("PoisionAttack3", playerPoisionAttackSE3.AcbAsset.Handle, playerPoisionAttackSE3.CueId, playertransform, false);
    }
    public static void PlayPlayerHibiscusAttackSE(Transform playertransform)
    {
        S_Sound.PlaySound("hibiscusAttack", playerHibiscusAttackSE.AcbAsset.Handle, playerHibiscusAttackSE.CueId, playertransform, false);
    }
    public static void PlayPlayerNemophilaAttackSE(Transform playertransform)
    {
        S_Sound.PlaySound("hibiscusAttack", playerNemophilaAttackSE.AcbAsset.Handle, playerNemophilaAttackSE.CueId, playertransform, false);
    }
    public static void PlayAcceleratorSE(Transform playertransform)
    {
        S_Sound.PlaySound("accelerator", acceleratorsSE.AcbAsset.Handle, acceleratorsSE.CueId, playertransform, false);
    }

    public static void PlayAcceleratorComboSE(Transform playertransform)
    {
        S_Sound.PlaySound("acceleratorCombo", acceleratorsComboSE.AcbAsset.Handle, acceleratorsComboSE.CueId, playertransform, false);
    }

    public static void PlayTreeSE1(Transform enemytransform)
    {
        //S_Sound.UpdateListenerPosition(S_CameraTransrom);
        //Debug.Log("treePos" + enemytransform.position);
        S_Sound.PlaySound("treeSE", treeSE1.AcbAsset.Handle, treeSE1.CueId, enemytransform, true);
    }

    public static void PlayTreeSE2(Transform enemytransform)
    {
        //Debug.Log("tree0");
        //S_Sound.UpdateListenerPosition(S_CameraTransrom);
        //Debug.Log("treePos2" + enemytransform.position);
        //Debug.Log(treeSE2.AcbAsset.Handle);
        //Debug.Log(treeSE2.CueId);
        S_Sound.PlaySound("treeSE2", treeSE2.AcbAsset.Handle, treeSE2.CueId, enemytransform, false);
    }


    public static void PlayboarVoiceSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("boarvoice", boarvoiceSE.AcbAsset.Handle, boarvoiceSE.CueId, enemytransform, true);
    }

    public static void PlayboarVoice2SE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("boarvoice2", boarvoice2SE.AcbAsset.Handle, boarvoice2SE.CueId, enemytransform, true);
        Debug.Log("soundboar");
    }

    public static void PlayboarWalkSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("boarWalkSE", boarwalkSE.AcbAsset.Handle, boarwalkSE.CueId, enemytransform, true);
    }

    public static void PlayCrowVoiceSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("crowVoiceSE", crowVoiceSE.AcbAsset.Handle, crowVoiceSE.CueId, enemytransform, true);
        Debug.Log("crowsound");
    }

    public static void PlayBearVoiceSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("bearVoice", bearVoiceSE.AcbAsset.Handle, bearVoiceSE.CueId, enemytransform, true);
    }

    public static void PlayEnemyDamageSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("dameageSE", enemyDamageSE.AcbAsset.Handle, enemyDamageSE.CueId, enemytransform,false);
    }

    public static void PlayEnemyDeatheSE(Transform enemytransform)
    {
        Debug.Log("deathSE");
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("deathSE", enemyDeathSE.AcbAsset.Handle, enemyDeathSE.CueId, enemytransform, true);
    }

    public static void PlayLila_L_thunderSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("LthunderVoice", lila_L_thunderSE.AcbAsset.Handle, lila_L_thunderSE.CueId, enemytransform, true);
    }

    public static void PlayLila_BardAttackSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lilaBardAttack", lila_bard_attackSE.AcbAsset.Handle, lila_bard_attackSE.CueId, enemytransform, true);
    }

    public static void PlayLila_WarpSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lilawarp", lila_WarpSE.AcbAsset.Handle, lila_WarpSE.CueId, enemytransform, true);
    }

    public static void PlayLila_WarpSE2(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lilawarp2", lila_WarpSE2.AcbAsset.Handle, lila_WarpSE2.CueId, enemytransform, true);
    }

    public static void PlayLila_ExplosionPoisionSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lilaexplosionPoision", lila_explosion_poisonSE.AcbAsset.Handle, lila_explosion_poisonSE.CueId, enemytransform, true);
    }

    public static void PlayLilaShockWaveSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("shockWave", lila_shockWaveSE.AcbAsset.Handle, lila_shockWaveSE.CueId, enemytransform, true);
    }

    public static void PlayLilaBomSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lila_bom", lila_BomSE.AcbAsset.Handle, lila_BomSE.CueId, enemytransform, true);
    }

    public static void PlayLilaMagicCircleSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lila_magiccircle", lila_magicCircleSE.AcbAsset.Handle, lila_magicCircleSE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingPoisonLakeSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("chantingPoisonLake", lila_chantingPoisonLakeSE.AcbAsset.Handle, lila_chantingPoisonLakeSE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingLThunderSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("chantingLthunder", lila_chantingLthunderSE.AcbAsset.Handle, lila_chantingLthunderSE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingBom1SE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lila_bom1", lila_chantingBom1SE.AcbAsset.Handle, lila_chantingBom1SE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingBom2SE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lila_bom2", lila_chantingBom2SE.AcbAsset.Handle, lila_chantingBom2SE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingBardSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("lila_bard", lila_chantingBardSE.AcbAsset.Handle, lila_chantingBardSE.CueId, enemytransform, true);
    }

    public static void PlayLilaChantingShockWaveSE(Transform enemytransform)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        S_Sound.PlaySound("chantingshockwave", lila_chantingShockWaveSE.AcbAsset.Handle, lila_chantingShockWaveSE.CueId, enemytransform, true);
    }

    

    public static void PlayLilaVoice(Transform enemytransform ,int voiceNum)
    {
        S_Sound.UpdateListenerPosition(S_CameraTransrom);
        if (voiceNum == 0)
        {
            S_Sound.PlaySound("lillavoice0", lila_Voice0.AcbAsset.Handle, lila_Voice0.CueId, enemytransform, false);
        }
        else if(voiceNum == 1)
        {
            S_Sound.PlaySound("lillavoice1", lila_Voice1.AcbAsset.Handle, lila_Voice1.CueId, enemytransform, false);
        }
        else if (voiceNum == 2)
        {
            S_Sound.PlaySound("lillavoice2", lila_Voice2.AcbAsset.Handle, lila_Voice2.CueId, enemytransform, false);
        }
        else if (voiceNum == 3)
        {
            S_Sound.PlaySound("lillavoice3", lila_Voice3.AcbAsset.Handle, lila_Voice3.CueId, enemytransform, false);
        }
        else if (voiceNum == 4)
        {
            S_Sound.PlaySound("lillavoice4", lila_Voice4.AcbAsset.Handle, lila_Voice4.CueId, enemytransform, false);
        }
    }
    public static void SEControl(string key, float value,float time,string aisacName,int number)
    {
        S_Sound.ChangeAisac(key, value, time, aisacName, number);
    }

   

}
