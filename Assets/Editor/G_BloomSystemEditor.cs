using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

//花を咲かせるためのシステム
[CustomEditor(typeof(G_FlowerBloomSystem))]
public class G_BloomSystemEditor : Editor
{
    private BloomSystemVarables bloomSystemData;

    //ReadOnlyDatas
    private readonly string[] tabButtonTexts = new string[3] { "地形解析", "開花", "その他" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (bloomSystemData==null)
        {
            bloomSystemData = new BloomSystemVarables();
        }
        if (bloomSystemData.bloomableAreas.Length==0)
        {
            bloomSystemData.selectedtabNumber = 0;
            EditorGUILayout.HelpBox($"解析データが入っていないと、{tabButtonTexts[1]}と{tabButtonTexts[2]}は解放されません。", MessageType.Warning);
            AnalysTerrainGUI();
            return;
        }

        GUILayout.BeginHorizontal();
        for(int i=0; i< tabButtonTexts.Length; i++)
        {
            if(GUILayout.Button(tabButtonTexts[i]))
            {
                bloomSystemData.selectedtabNumber = i;
            }
        }
        GUILayout.EndHorizontal();

        switch (bloomSystemData.selectedtabNumber)
        {
            case 0:
                AnalysTerrainGUI();
                break;
            case 1:
                BloomButton();
                break;
            case 2:
                OtherButton();
                break;
            default:
                break;
        }
    }


    //地形解析が選ばれているとき
    void AnalysTerrainGUI()
    {
        //開花範囲スライダー
        bloomSystemData.analysDensity = EditorGUILayout.Slider(new GUIContent("解析エリア半径", "開花エリアグループの半径を決めます。\n小さいほど正確ですが高負荷になります。"), bloomSystemData.analysDensity,0.01f,10f);

        //開花可能最大角度スライダー
        bloomSystemData.bloomableMaxAngle = EditorGUILayout.Slider(new GUIContent("開花可能最大角度", $"Terrainの角度が{bloomSystemData.bloomableMaxAngle}度以上の場合、開花しません。"), bloomSystemData.bloomableMaxAngle, 0f, 90f);

        //開花Layerスライダー
        TerrainData data= GetParentTerrain().terrainData;
        bloomSystemData.bloomableTerrainLayer = EditorGUILayout.IntSlider(new GUIContent("開花レイヤー", "開花可能なTerrainLayerを数値で選択します。"), bloomSystemData.bloomableTerrainLayer, 0, data.terrainLayers.Length-1);

        //解析ボタン
        if (GUILayout.Button("解析開始"))
        {
            AnalysTerrain();
        }

        /// <summary>
        /// 地形解析を行う。
        /// </summary>
        void AnalysTerrain()
        {
            Terrain terrain = GetParentTerrain();
            TerrainData terrainData = terrain.terrainData;

            bool setZero = false;
            float length = 2f * bloomSystemData.analysDensity;
            float sinLength = length / Mathf.Sqrt(2f);

            List<Vector3> positions = new List<Vector3>();

            float add = (sinLength / terrainData.size.z) * (length / terrainData.size.x);
            float process = 0f;

            for (float z = bloomSystemData.analysDensity; z < terrainData.size.z; z += sinLength)
            {
                for (float x = (setZero ? 0f : bloomSystemData.analysDensity); x < terrainData.size.x; x += length)
                {
                    Vector2 axis = new Vector2(x, z);
                    Vector3 position = new Vector3();
                    if(BloomablePosition(axis, out position))
                    {
                        positions.Add(position);
                    }
                    process += add;
                    EditorUtility.DisplayProgressBar("地形解析中", "地形を解析中です...  "+process.ToString("P"), process);
                }
                setZero = !setZero;
            }
            bloomSystemData.bloomableAreas = positions.ToArray();
            Debug.Log($"解析完了\n開花エリアは全部で{bloomSystemData.bloomableAreas.Length}です。");
            EditorUtility.ClearProgressBar();
        }
    }

    //開花が選ばれている時
    void BloomButton()
    {
        G_FlowerBloomSystem bloomSystem = target as G_FlowerBloomSystem;

        bloomSystemData.setBloomYears = GUILayout.Toggle(bloomSystemData.setBloomYears, "開花年数を指定する");
        if (bloomSystemData.setBloomYears)
        {
            bloomSystemData.bloomYear = EditorGUILayout.IntSlider("開花年数",bloomSystemData.bloomYear, 1, 10);
        }

        if (bloomSystemData.isBaked)
        {
            EditorGUILayout.HelpBox("焼き付け状態での子孫繁栄は行えません。", MessageType.Warning);
            return;
        }

        if(bloomSystem.transform.childCount == 0)
        {
            EditorGUILayout.HelpBox("開花を行う時は最低でも一輪の花が必要です。", MessageType.Error);
            return;
        }

        bloomSystemData.areaDensity = 1f / EditorGUILayout.Slider(new GUIContent("開花密度", "開花エリアグループ内の密度を示します。最大最小密度はエリアサイズに依存します。"), 1f / bloomSystemData.areaDensity, 0.1f, 10f);
        bloomSystemData.areaRandomize = EditorGUILayout.Slider(new GUIContent("開花乱数値", "開花地点のランダム性を決めます。値が大きいほど、開花地点がばらつきます。"), bloomSystemData.areaRandomize, 0f, 1f);

        EditorGUILayout.BeginHorizontal();
        {
            bloomSystemData.minFlowerScale = EditorGUILayout.FloatField(bloomSystemData.minFlowerScale);
            bloomSystemData.maxFlowerScale = EditorGUILayout.FloatField(bloomSystemData.maxFlowerScale);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.MinMaxSlider(new GUIContent("開花サイズ", "開花する花のサイズの最小値と最大値を指定します。"), ref bloomSystemData.minFlowerScale, ref bloomSystemData.maxFlowerScale, 0f, 2f);


        if (GUILayout.Button("子孫繁栄"))
        {
            Bloom();
        }

        void Bloom()
        {
            List<GameObject> parents = new List<GameObject>();
            foreach (Transform child in bloomSystem.transform)
            {
                parents.Add(child.gameObject);
            }

            int areaAmount = bloomSystemData.bloomableAreas.Length;
            int selectedAreaAmount = 0;
            int year = 0;
            int lastYearAmount = 0;

            List<GameObject>[] bloomableFlowerObjects = (new List<GameObject>[areaAmount]).Select(list => new List<GameObject>()).ToArray();

            bool[] isSelecteds = new bool[areaAmount];

            int whileAmount = 0;
            while ((selectedAreaAmount<areaAmount) && (!bloomSystemData.setBloomYears || (year < bloomSystemData.bloomYear)))
            {
                List<G_Flower> flowers = new List<G_Flower>();

                //親となる花オブジェクトの取得
                foreach (Transform child in bloomSystem.transform)
                {
                    G_Flower flower = child.GetComponent<G_Flower>();
                    if (!flower)
                    {
                        Debug.LogError($"GameObject {child.gameObject.name}にはG_FlowerComponentがセットされていません。", this);
                        return;
                    }
                    flowers.Add(flower);
                }

                float analysProcess = 0f;
                float analysAddProcess = 1f / (float)(bloomSystemData.bloomableAreas.Length * flowers.Count);
                //広がる距離を参考にしてエリアに花を分配する
                for (int i = 0; i < bloomSystemData.bloomableAreas.Length; i++)
                {
                    if (isSelecteds[i])
                    {
                        continue;
                    }
                    Vector3 areaPosition = bloomSystemData.bloomableAreas[i];

                    //各花との距離を確認

                    foreach (G_Flower flower in flowers)
                    {
                        float sqrSpreadDistance = Mathf.Pow(flower.spreadDistance, 2f);
                        float sqrDistance = Vector3.SqrMagnitude(flower.transform.position - areaPosition);

                        if (sqrDistance <= sqrSpreadDistance)
                        {
                            bloomableFlowerObjects[i].Add(flower.gameObject);
                            if (!isSelecteds[i])
                            {
                                selectedAreaAmount++;
                                isSelecteds[i] = true;
                            }
                        }
                        analysProcess += analysAddProcess;
                        EditorUtility.DisplayProgressBar("子孫繁栄", $"{year}年目 開花グループの解析中...  確定エリア数: {selectedAreaAmount} / {areaAmount}" + analysProcess.ToString("P"), analysProcess);
                    }
                }
                if(lastYearAmount == selectedAreaAmount)
                {
                    break;
                }
                else
                {
                    lastYearAmount = selectedAreaAmount;
                }
                whileAmount++;
                year++;
                if(whileAmount >= 10)
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }

            //開花させる
            float areaSize = bloomSystemData.analysDensity;
            float density = bloomSystemData.areaDensity;

            float bloomProcess = 0f;
            float addProcess = 1f / (float)bloomSystemData.bloomableAreas.Length;
            int can = 0, cant = 0;
            for (int i = 0; i < bloomSystemData.bloomableAreas.Length; i++)
            {
                GameObject[] flowerObjects = bloomableFlowerObjects[i].ToArray();
                if (flowerObjects.Length == 0)
                {
                    cant++;
                    Debug.Log("flowerObjects.Length == 0");
                    continue;
                }

                Vector3 centerPosition = bloomSystemData.bloomableAreas[i];
                Vector2 centerAxis = new Vector2(centerPosition.x, centerPosition.z);

                GameObject bloomArea = new GameObject($"FlowerGroup({i})");
                bloomArea.tag = "Flower";
                bloomArea.layer = bloomSystemData.flowerLayerNumber;
                bloomArea.transform.parent = bloomSystem.transform;
                bloomArea.transform.position = centerPosition;

                var collider =  bloomArea.AddComponent<SphereCollider>();
                var flowerGroup =  bloomArea.AddComponent<G_FlowerGroup>();
                bloomArea.AddComponent<G_LOD>();
                collider.radius = bloomSystemData.analysDensity;
                collider.isTrigger = true;

                flowerGroup.flowerLayer = bloomSystemData.flowerLayerNumber;
                flowerGroup.bloomedFlowerLayer = bloomSystemData.bloomedFlowerLayerNumber;


                for (float z = -areaSize; z < areaSize; z += density)
                {
                    for(float x = -areaSize; x < areaSize; x += density)
                    {
                        Vector2 nonRandomAxis = centerAxis + new Vector2(x, z);
                        Vector2 axis = nonRandomAxis + (bloomSystemData.areaRandomize * UnityEngine.Random.insideUnitCircle);
                        Terrain terrain = GetParentTerrain();
                        Vector3 terrainPosition = terrain.transform.position;
                        Vector2 terrainAxis = new Vector2(terrainPosition.x, terrainPosition.z);

                        Vector3 position;
                        try
                        {
                            if (!BloomablePosition(axis - terrainAxis, out position))
                            {
                                continue;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        can++;
                        position = new Vector3(axis.x, position.y, axis.y);
                        Quaternion rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
                        Vector3 localScale = UnityEngine.Random.Range(bloomSystemData.minFlowerScale, bloomSystemData.maxFlowerScale) * Vector3.one;

                        int bloomFlowerNumber = UnityEngine.Random.Range(0, flowerObjects.Length);

                        GameObject bloomFlowerObject = flowerObjects[bloomFlowerNumber];
                        G_Flower bloomFlower = bloomFlowerObject.GetComponent<G_Flower>();
                        if(bloomFlower.flowerType == G_Flower.FlowerList.sunflower)
                        {
                            rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(85f, 95f), 0f);
                        }

                        GameObject flowerObject =  Instantiate(bloomFlowerObject, bloomArea.transform);
                        flowerObject.name = bloomFlowerObject.name;
                        Transform flowerTransform = flowerObject.transform;
                        flowerTransform.position = position;
                        flowerTransform.rotation = rotation;
                        flowerTransform.localScale = localScale;

                    }
                }
                bloomProcess += addProcess;
                EditorUtility.DisplayProgressBar("子孫繁栄", "花を配置中...  " + bloomProcess.ToString("P"), bloomProcess);
                can++;
            }
            Debug.Log($"can:{can}\tcant:{cant}\t合計:{can+cant}");
            EditorUtility.ClearProgressBar();
            EditorUtility.SetDirty(bloomSystem);
        }
    }

    void OtherButton()
    {
        G_FlowerBloomSystem bloomSystem = target as G_FlowerBloomSystem;

        if (GUILayout.Button(new GUIContent("全ての花を表示", "全ての花を可視化します。実際のゲームでは使用できません。")))
        {
            float process = 0f;
            float addProcess = 1f / bloomSystem.transform.childCount;
            foreach (Transform child in bloomSystem.transform)
            {
                var flowerGroup = child.GetComponent<G_FlowerGroup>();
                if (!flowerGroup)
                {
                    continue;
                }
                foreach (Transform grandchild in child)
                {
                    grandchild.gameObject.SetActive(true);
                }
                process += addProcess;
                EditorUtility.DisplayProgressBar("花を表示中", "花をビジュアライズ化しています...  " + process.ToString("P"), process);
            }
            EditorUtility.SetDirty(bloomSystem);
            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button(new GUIContent("全ての花を非表示", "全ての花を非表示にします。ゲームを実行する前に実行してください。")))
        {
            float process = 0f;
            float addProcess = 1f / bloomSystem.transform.childCount;
            foreach (Transform child in bloomSystem.transform)
            {
                var flowerGroup = child.GetComponent<G_FlowerGroup>();
                if (!flowerGroup)
                {
                    continue;
                }
                foreach (Transform grandchild in child)
                {
                    grandchild.gameObject.SetActive(false);
                }
                process += addProcess;
                EditorUtility.DisplayProgressBar("花を表示中", "花をビジュアライズ化しています...  " + process.ToString("P"), process);
            }
            EditorUtility.SetDirty(bloomSystem);
            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button(new GUIContent("エリアグループの削除", "グループ化されたエリアを全て削除します。\n現時点では名前に依存します。")))
        {
            float process = 0f;
            float addProcess = 1f / bloomSystem.transform.childCount;
            foreach (Transform child in bloomSystem.transform)
            {
                if (child.gameObject.name.Contains("FlowerGroup"))
                {
                    DestroyImmediate(child.gameObject);
                }
                process += addProcess;
                EditorUtility.DisplayProgressBar("エリアグループ削除", "エリアグループを削除しています...  " + process.ToString("P"), process);
            }
            EditorUtility.SetDirty(bloomSystem);
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// その地点に開花可能かを調べます。
    /// </summary>
    /// <param name="axis">咲く予定のxz二次元座標を入力</param>
    /// <param name="position">Teiianを参考にした高さを追加して座標を返します</param>
    /// <returns>開花可能な場合はtrueを返します。</returns>
    bool BloomablePosition(Vector2 axis, out Vector3 position)
    {
        Terrain terrain;
        position =  GetTerrainPosition(axis, out terrain);
        TerrainData terrainData = terrain.terrainData;
        int terrainDetail = terrainData.detailResolution;
        Vector2 detailConstant = new Vector2(1f/terrainData.size.x, 1f/terrainData.size.z);

        Vector2 detailAxis = Vector2.Scale(axis, detailConstant);

        //Terrainの角度を計測
        Vector3 normal = terrainData.GetInterpolatedNormal(detailAxis.x,detailAxis.y);
        float angle = Mathf.Abs(Vector3.Angle(normal, Vector3.up));
        if (angle > bloomSystemData.bloomableMaxAngle)
        {
            return false;
        }

        //TerrainTextureを確認
        int textureResolution = terrainData.heightmapResolution -1;
        Vector2 heightMapDetailConstant = new Vector2(1f / terrainData.size.x, 1f / terrainData.size.z) * textureResolution;
        Vector2Int texutureMap = Vector2Int.RoundToInt(Vector2.Scale(heightMapDetailConstant, axis));
        float[,,] splatMapData = terrainData.GetAlphamaps(texutureMap.x, texutureMap.y, 1, 1);
        float maxWeight = 0f;
        int dominantTextureIndex = -1;
        for (int i = 0; i < terrainData.terrainLayers.Length; i++)
        {
            if (splatMapData[0, 0, i] > maxWeight)
            {
                maxWeight = splatMapData[0, 0, i];
                dominantTextureIndex = i;
            }
        }
        if (bloomSystemData.bloomableTerrainLayer != dominantTextureIndex)
        {
            return false;
        }
        //上部にWaterがあるかどうかのチェックを行う
        float rayHeight = terrain.transform.position.y + terrainData.size.y;
        Vector3 rayStartPosition = position + (Vector3.up * rayHeight);
        Ray ray = new Ray(rayStartPosition, Vector3.down);
        var hits = Physics.SphereCastAll(ray, bloomSystemData.analysDensity).OrderBy(hit => hit.distance);
        foreach(var hit in hits)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                return true;
            }
            else if (hit.collider.gameObject.CompareTag("Water"))
            {
                return false;
            }
        }

        return false;
    }

    private Terrain GetParentTerrain()
    {
        return (target as G_FlowerBloomSystem).transform.parent.GetComponent<Terrain>();
    }


    /// <summary>
    /// 二次元座標からTerrainを参照した三次元座標を返します。
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="terrain"></param>
    /// <returns></returns>
    Vector3 GetTerrainPosition(Vector2 axis, out Terrain terrain)
    {
        terrain = GetParentTerrain();
        TerrainData terrainData = terrain.terrainData;
        Vector2 detailConstant = new Vector2(1f / terrainData.size.x, 1f / terrainData.size.z);

        Vector2 detailAxis = Vector2.Scale(axis, detailConstant);
        float y = terrainData.GetInterpolatedHeight(detailAxis.x, detailAxis.y);
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 position = terrainPosition + new Vector3(axis.x, y, axis.y);
        return position;
    }

    void OnEnable()
    {

        //データが存在する場合は読み込む
        if (File.Exists(SavePath()))
        {

            using (StreamReader sr = new StreamReader(SavePath()))
            {
                if (bloomSystemData == null)
                {
                    bloomSystemData = new BloomSystemVarables();
                }
                JsonUtility.FromJsonOverwrite(sr.ReadToEnd(), bloomSystemData);
            }
        }
        else
        {
            bloomSystemData = new BloomSystemVarables();
        }
    }

    private void OnDisable()
    {

        //データを保存
        string path = SavePath();
        using (StreamWriter sw = new StreamWriter(path, false))
        {
            string jsonstr = JsonUtility.ToJson(bloomSystemData, false);
            sw.Write(jsonstr);
            sw.Flush();
        }
    }

    /// <summary>
    /// Edtorのデータをセーブする時のパス
    /// </summary>
    /// <returns></returns>
    string SavePath() => $"Assets/EditorWindowSaveData/{GetType()}.json";

    [Serializable]
    private class BloomSystemVarables
    {
        public int selectedtabNumber = 0;
        public float analysDensity = 0.5f;
        public float bloomableMaxAngle = 60f;
        public int bloomableTerrainLayer = 0;

        public bool setBloomYears = false;
        public int bloomYear = 1;
        public float areaDensity = 0.05f;
        public float areaRandomize = 0.1f;
        public float minFlowerScale = 0.8f;
        public float maxFlowerScale = 1.2f;

        public bool isBaked = false;
        public int flowerLayerNumber = 6, bloomedFlowerLayerNumber = 10;

        public Vector3[] bloomableAreas = new Vector3[0];

        public GameObject[] parentObjects = new GameObject[0];
    }
}
