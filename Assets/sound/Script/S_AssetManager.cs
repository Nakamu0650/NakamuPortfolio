using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AssetManager : MonoBehaviour
{

    [SerializeField] CriWare.Assets.CriAtomCueReference menunoise;
    [SerializeField] CriWare.Assets.CriAtomCueReference gamestart;
    [SerializeField] CriWare.Assets.CriAtomCueReference menuOpen;
    [SerializeField] CriWare.Assets.CriAtomCueReference menuClose;
    [SerializeField] CriWare.Assets.CriAtomCueReference StartselectSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference notStartSelectedSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference selectSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference textSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference targetCameraOnSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference targetCameraOffSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerDamegeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerWalkSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerRunSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerBrakeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerLandingSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerJumpSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerFlySE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerUmbrellaTakeOutSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerUmbrellaPutAwaySE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerUmbrellaAttackHiSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerAttackPrepareSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerAvoidSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerAttackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerEquipSE1;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerEquipSE2;
    [SerializeField] CriWare.Assets.CriAtomCueReference flowersBloomSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference flowersBloomSE2;
    [SerializeField] CriWare.Assets.CriAtomCueReference lackOfEnergySE;
    [SerializeField] CriWare.Assets.CriAtomCueReference energyGetSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference roseAttackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference RoseAttackChargeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference roseAttackhitSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference roseRotationAttackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference roseRotationAttackStopSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference sunflowerLaserChargeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference sunflowerLaserShot1SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference sunflowerLaserShot2SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference sunflowerLaserShot3SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference sunflowerLaserHit;
    [SerializeField] CriWare.Assets.CriAtomCueReference balsamChargeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference balsamDistanceSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference balsamHitSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference balsamHitLandingSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosisSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosisGentianSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosisCherryBlossomsSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosishibiscusSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosisNemophilaSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerMetamorphosisPoisionSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerPoisionAttackSE3;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerPoisionAttackSE1;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerPoisionAttackSE2;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerHibiscusAttackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference playerNemophilaAttackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference shieldSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference shieldBreakeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference shieldParySE;
    [SerializeField] CriWare.Assets.CriAtomCueReference recoverySE;
    [SerializeField] CriWare.Assets.CriAtomCueReference acceleratorsSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference acceleratorsComboSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference treeSE1;
    [SerializeField] CriWare.Assets.CriAtomCueReference treeSE2;
    [SerializeField] CriWare.Assets.CriAtomCueReference boarvoiceSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference boarvoice2SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference boarwalkSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference crowVoiceSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference bearVoiceSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference enemyDamageSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference enemyDeathSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_L_thunderSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_bard_attackSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_WarpSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_WarpSE2;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_explosion_poisonSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_shockWaveSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_BomSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_magicCircleSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingPoisonLakeSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingLthunderSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingBom1SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingBom2SE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingBardSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_chantingShockWaveSE;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_Voice0;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_Voice1;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_Voice2;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_Voice3;
    [SerializeField] CriWare.Assets.CriAtomCueReference lila_Voice4;
    [SerializeField] Transform Lisener;
    [SerializeField] Transform CameraTransrom;

    [SerializeField] List<string> playerWalkAisacName = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        S_SEManager.menunoise = menunoise;
        S_SEManager.gamestart = gamestart;
        S_SEManager.menuOpen = menuOpen;
        S_SEManager.menuClose = menuClose;
        S_SEManager.StartselectSE = StartselectSE;
        S_SEManager.notStartSelectedSE = notStartSelectedSE;
        S_SEManager.selectSE = selectSE;
        S_SEManager.textSE = textSE;
        S_SEManager.targetCameraOnSE = targetCameraOnSE;
        S_SEManager.targetCameraOffSE = targetCameraOffSE;
        S_SEManager.S_Lisener = Lisener;
        S_SEManager.S_CameraTransrom = CameraTransrom;
        S_SEManager.playerDamegeSE = playerDamegeSE;
        S_SEManager.playerWalkSE = playerWalkSE;
        S_SEManager.playerLandingSE = playerLandingSE;
        S_SEManager.playerJumpSE = playerJumpSE;
        S_SEManager.playerRunSE = playerRunSE;
        S_SEManager.playerFlySE = playerFlySE;
        S_SEManager.playerBrakeSE = playerBrakeSE;
        S_SEManager.playerUmbrellaTakeOutSE = playerUmbrellaTakeOutSE;
        S_SEManager.playerUmbrellaPutAwaySE = playerUmbrellaPutAwaySE;
        S_SEManager.playerUmbrellaAttackHiSE = playerUmbrellaAttackHiSE;
        S_SEManager.playerAttackPrepareSE = playerAttackPrepareSE;
        S_SEManager.playerAvoidSE = playerAvoidSE;
        S_SEManager.playerAttackSE = playerAttackSE;
        S_SEManager.playerEquipSE1 = playerEquipSE1;
        S_SEManager.playerEquipSE2 = playerEquipSE2;
        S_SEManager.flowersBloomSE = flowersBloomSE;
        S_SEManager.flowersBloomSE2 = flowersBloomSE2;
        S_SEManager.lackOfEnergySE = lackOfEnergySE;
        S_SEManager.energyGetSE = energyGetSE;
        S_SEManager.roseAttackSE = roseAttackSE;
        S_SEManager.RoseAttackChargeSE = RoseAttackChargeSE;
        S_SEManager.roseAttackhitSE = roseAttackhitSE;
        S_SEManager.roseRotationAttackSE = roseRotationAttackSE;
        S_SEManager.roseRotationAttackStopSE = roseRotationAttackStopSE;
        S_SEManager.sunflowerLaserChargeSE = sunflowerLaserChargeSE;
        S_SEManager.sunflowerLaserShot1SE = sunflowerLaserShot1SE;
        S_SEManager.sunflowerLaserShot2SE = sunflowerLaserShot2SE;
        S_SEManager.sunflowerLaserShot3SE = sunflowerLaserShot3SE;
        S_SEManager.sunflowerLaserHit = sunflowerLaserHit;
        S_SEManager.balsamChargeSE = balsamChargeSE;
        S_SEManager.balsamDistanceSE = balsamDistanceSE;
        S_SEManager.balsamHitLandingSE = balsamHitLandingSE;
        S_SEManager.balsamHitSE = balsamHitSE;
        S_SEManager.playerMetamorphosisSE = playerMetamorphosisSE;
        S_SEManager.playerMetamorphosisGentianSE = playerMetamorphosisGentianSE;
        S_SEManager.playerMetamorphosisCherryBlossomsSE = playerMetamorphosisCherryBlossomsSE;
        S_SEManager.playerMetamorphosishibiscusSE = playerMetamorphosishibiscusSE;
        S_SEManager.playerMetamorphosisNemophilaSE = playerMetamorphosisNemophilaSE;
        S_SEManager.playerMetamorphosisPoisionSE = playerMetamorphosisPoisionSE;
        
        S_SEManager.playerPoisionAttackSE1 = playerPoisionAttackSE1;
        S_SEManager.playerPoisionAttackSE2 = playerPoisionAttackSE2;
        S_SEManager.playerPoisionAttackSE3 = playerPoisionAttackSE3;
        S_SEManager.playerHibiscusAttackSE = playerHibiscusAttackSE;
        S_SEManager.playerNemophilaAttackSE = playerNemophilaAttackSE;
        S_SEManager.recoverySE = recoverySE;
        S_SEManager.shieldSE = shieldSE;
        S_SEManager.shieldBreakeSE = shieldBreakeSE;
        S_SEManager.shieldParySE = shieldParySE;
        S_SEManager.acceleratorsSE = acceleratorsSE;
        S_SEManager.acceleratorsComboSE = acceleratorsComboSE;
        S_SEManager.treeSE1 = treeSE1;
        S_SEManager.treeSE2 = treeSE2;
        S_SEManager.boarvoiceSE = boarvoiceSE;
        S_SEManager.boarvoice2SE = boarvoice2SE;
        S_SEManager.boarwalkSE = boarwalkSE;
        S_SEManager.crowVoiceSE = crowVoiceSE;
        S_SEManager.bearVoiceSE = bearVoiceSE;
        S_SEManager.enemyDamageSE = enemyDamageSE;
        S_SEManager.enemyDeathSE = enemyDeathSE;
        S_SEManager.lila_L_thunderSE = lila_L_thunderSE;
        S_SEManager.lila_bard_attackSE = lila_bard_attackSE;
        S_SEManager.lila_WarpSE = lila_WarpSE;
        S_SEManager.lila_WarpSE2 = lila_WarpSE2;
        S_SEManager.lila_explosion_poisonSE = lila_explosion_poisonSE;
        S_SEManager.lila_shockWaveSE = lila_shockWaveSE;
        S_SEManager.lila_BomSE = lila_BomSE;
        S_SEManager.lila_magicCircleSE = lila_magicCircleSE;
        S_SEManager.lila_chantingPoisonLakeSE = lila_chantingPoisonLakeSE;
        S_SEManager.lila_chantingLthunderSE = lila_chantingLthunderSE;
        S_SEManager.lila_chantingBom1SE = lila_chantingBom1SE;
        S_SEManager.lila_chantingBom2SE = lila_chantingBom2SE;
        S_SEManager.lila_chantingBardSE = lila_chantingBardSE;
        S_SEManager.lila_chantingShockWaveSE = lila_chantingShockWaveSE;
        S_SEManager.lila_Voice0 = lila_Voice0;
        S_SEManager.lila_Voice1 = lila_Voice1;
        S_SEManager.lila_Voice2 = lila_Voice2;
        S_SEManager.lila_Voice3 = lila_Voice3;
        S_SEManager.lila_Voice4 = lila_Voice4;
       
        S_SEManager.playerWalkAisacName = playerWalkAisacName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
