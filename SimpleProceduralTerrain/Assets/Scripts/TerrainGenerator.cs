using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TerrainGenerator : MonoBehaviour 
{
	//Splat
	SplatPrototype[] splatPrototypes;

	//Tree
	TreePrototype[] treePrototypes;
	public GameObject[] treePrefabs = new GameObject[3];

	//Details
	DetailPrototype[] detailPrototypes;

	//Perlin
	Noise groundNoise, mountainNoise, treeNoise, detailNoise;

	//seeds
	public int groundSeed = 0;
	public int mountainSeed = 1;
	public int treeSeed = 2;
	public int detailSeed = 3;

	// Frequency
	public float groundFrq = 800.0f;
	public float  mountainFrq = 1200.0f;
	public float  treeFrq = 400.0f;
	public float  detailFrq = 100.0f;


	//Terrain settings
	public int tilesX = 2; //Number of terrain tiles on the x axis
	public int tilesZ = 2; //Number of terrain tiles on the z axis

	Terrain current;
	Terrain left;
	Terrain right;
	Terrain top;
	Terrain bottom;
	public float pixelMapError = 6.0f; 
	public float baseMapDist = 1000.0f; //The distance at which the low res base map will be drawn. Decrease to increase performance

	//Terrain data settings
	public int heightMapSize = 513; 
	public int alphaMapSize = 1024; 
	public int terrainSize = 2048;
	public int terrainHeight = 512;
	public int detailMapSize = 512; 

	//Tree settings
	public int treeSpacing = 32; //spacing between trees
	public float treeDistance = 2000.0f; //The distance at which trees will no longer be drawn
	public float treeBillboardDistance = 400.0f; 
	public float treeCrossFadeLength = 20.0f; 
	public int treeMaximumFullLODCount = 400; //The maximum number of trees that will be drawn in a certain area. 


	//Detail settings
	public int detailObjectDistance = 400; //The distance at which details will no longer be drawn
	public float detailObjectDensity = 4.0f; 
	public int detailResolutionPerPatch = 32; 
	public float wavingGrassStrength = 0.4f;
	public float wavingGrassAmount = 0.2f;
	public float wavingGrassSpeed = 0.4f;
	public Color wavingGrassTint = Color.white;


	Terrain[] terrain;
	Vector2 offset;
	public GameObject player;
	Vector3 startPos;
	int x = 0;
	int z = 0;
	ArrayList listX = new ArrayList();
	ArrayList listZ = new ArrayList();
	
	void Start() 
	{
		groundNoise = new Noise();
		groundNoise.setPermutations (groundSeed);

		mountainNoise = new Noise();
		mountainNoise.setPermutations (mountainSeed);

		treeNoise = new Noise();
		treeNoise.setPermutations (treeSeed);

		detailNoise = new Noise();
		detailNoise.setPermutations (detailSeed);
		
		if(!Mathf.IsPowerOfTwo(heightMapSize-1))
		{
			heightMapSize = Mathf.ClosestPowerOfTwo(heightMapSize)+1;
		}
		
		if(!Mathf.IsPowerOfTwo(alphaMapSize))
		{
			alphaMapSize = Mathf.ClosestPowerOfTwo(alphaMapSize);
		}
		
		if(!Mathf.IsPowerOfTwo(detailMapSize))
		{
			detailMapSize = Mathf.ClosestPowerOfTwo(detailMapSize);
		}
		
		if(detailResolutionPerPatch < 8)
		{
			detailResolutionPerPatch = 8;
		}
		

		
//		terrain = new Terrain[tilesX,tilesZ];
		
		//this will center terrain at origin



		//populate prototypes
		if(splatDataCreated()){
			Debug.Log("Splats Created");
			if (treeDataCreated ()) {
				Debug.Log ("Trees Created");
				if(detailsDataCreated()){
					Debug.Log("Details Created");
				}
			}
		}

		Terrain[,] terrain = new Terrain[tilesX, tilesZ];


//		for(int x = 0; x < tilesX; x++)
//		{
//			for(int z = 0; z < tilesZ; z++)
//			{
//				GenerateTerrains(x, z, terrain[x,z] );
//			}
//		}

		// Get Player's initial position
		startPos = Vector3.zero;
		Debug.Log (startPos);
		
		// Generate 4 terrains at the beginning
		current = GenerateTerrains(x, z, current );
		
		if(left == null) {
			int leftX = x-1;
			left = GenerateTerrains(leftX, z, left );
		}
		if(right == null) {
			int rightX = x+1;
			right = GenerateTerrains(rightX, z, right );
		}
		if(top == null) {
			int topZ = z+1;
			top = GenerateTerrains(x, topZ, top );
		}
	    if(bottom == null) {
			int botZ = z-1;
			bottom = GenerateTerrains(x, botZ, bottom );
		}

		listX.Add (x);
		listZ.Add (z);


		Debug.Log (listX.Contains(x));
	}

	public void Update(){
		Vector3 comparePos = startPos;
		//Forward
		if ((player.transform.position.z - comparePos.z) >= (terrainSize * 0.5f)) {

			if(!listZ.Contains(z)){ 
				bottom = current;
				left = null;
				right = null;
				top = null;
				listZ.Add(z+1);
				z = z+1;
				comparePos.z = player.transform.position.z;
			}
					}
		// Right
		if ((player.transform.position.x - comparePos.x) >= (terrainSize * 0.5f) ){
			if(!listX.Contains(x)){
				bottom = null;
				left = current;
				right = null;
				top = null;
				listX.Add(x+1);
				x = x+1;
				comparePos.x = player.transform.position.z;
			}
		}
		//Bottom
		if ((comparePos.z - player.transform.position.z) >= (terrainSize * 0.5f)) {
			if(!listZ.Contains(z)){
				bottom = null;
				left = null;
				right = null;
				top = current;
				listZ.Add(z-1);
				z = z-1;
				comparePos.z = player.transform.position.z;
			}
		}
		//Left
		if ((comparePos.x - player.transform.position.x) >= (terrainSize * 0.5f)) {
			if(!listX.Contains(x)){
				bottom = null;
				left = null;
				right = current;
				top = null;
				listX.Add (x-1);
				x = x-1;
				comparePos.x = player.transform.position.x;
			}
		}

		if(left == null) {
			int leftX = x-1;
			left = GenerateTerrains(leftX, z, left );
		}
		if(right == null) {
			int rightX = x+1;
			right = GenerateTerrains(rightX, z, right );
		}
		if(top == null) {
			int topZ = z+1;
			top = GenerateTerrains(x, topZ, top );
		}
	    if(bottom == null) {
			int botZ = z-1;
			bottom = GenerateTerrains(x, botZ, bottom );
		}
	}

	public Terrain GenerateTerrains(int x, int z, Terrain t){
	
		float[,] htmap = new float[heightMapSize,heightMapSize];

		CreateBasicTerrain(htmap, x, z);
		TerrainData terrainData = new TerrainData();
		
		terrainData.heightmapResolution = heightMapSize;
		terrainData.SetHeights(0, 0, htmap);
		terrainData.size = new Vector3(terrainSize, terrainHeight, terrainSize);
		terrainData.splatPrototypes = splatPrototypes;
		terrainData.treePrototypes = treePrototypes;
		terrainData.detailPrototypes = detailPrototypes;
		
		AddAlphaMap(terrainData);
		
		t = Terrain.CreateTerrainGameObject(terrainData).GetComponent<Terrain>();
		t.transform.position = new Vector3(terrainSize*x -(terrainSize*0.5f), 0, terrainSize*z - (terrainSize*0.5f));
		t.heightmapPixelError = pixelMapError;
		t.basemapDistance = baseMapDist;
		
		
		AddTrees(t, x, z);
		AddDetailMap(t, x, z);			
		return t;
	}

	/// <summary>
	/// Creates the basic terrain.
	/// At this point only plane & mountains will be generated
	/// </summary>
	/// <param name="htmap">Htmap.</param>
	/// <param name="tileX">Tile x = Current Terrain's x</param>
	/// <param name="tileZ">Tile z = Current terrain's Z</param>
	void CreateBasicTerrain(float[,] htmap, int tileX, int tileZ)
	{
		float ratio = (float)terrainSize/(float)heightMapSize;
		
		for(int x = 0; x < heightMapSize; x++)
		{
			for(int z = 0; z < heightMapSize; z++)
			{
				float worldPosX = (x+tileX*(heightMapSize-1))*ratio;
				float worldPosZ = (z+tileZ*(heightMapSize-1))*ratio;

				float mountains = Mathf.Max(0.0f,mountainNoise.FractalNoise(worldPosX, worldPosZ, 6, mountainFrq, 0.8f));
				
				float plain = groundNoise.FractalNoise(worldPosX, worldPosZ, 4, groundFrq, 0.1f) + 0.1f;
				
				htmap[z,x] = plain+mountains;
			}
		}
	}

	/// <summary>
	/// Adds the alpha map.
	/// </summary>
	/// <param name="terrainData">Terrain data.</param>
	void AddAlphaMap(TerrainData terrainData) 
	{
        float[,,] map  = new float[alphaMapSize, alphaMapSize, 2];
		
		Random.seed = 0;
        
        for(int x = 0; x < alphaMapSize; x++) 
		{
            for (int z = 0; z < alphaMapSize; z++) 
			{
                // Get the normalized terrain coordinate that
                // corresponds to the the point.
                float normX = x * 1.0f / (alphaMapSize - 1);
                float normZ = z * 1.0f / (alphaMapSize - 1);
                
                // Get the steepness value at the normalized coordinate.
                float angle = terrainData.GetSteepness(normX, normZ);
                
                // Divide by 90 to get an alpha blending value in the range 0..1.
                float frac = angle / 90.0f;
                map[z, x, 0] = frac;
                map[z, x, 1] = 1.0f - frac;
				
            }
        }
        
		terrainData.alphamapResolution = alphaMapSize;
        terrainData.SetAlphamaps(0, 0, map);
    }

	/// <summary>
	/// Adds the trees with trespace distance away from each other.
	/// </summary>
	/// <param name="terrain">Terrain.</param>
	/// <param name="tileX">Tile x.</param>
	/// <param name="tileZ">Tile z.</param>
	void AddTrees(Terrain terrain, int tileX, int tileZ)
	{
		Random.seed = 0;
	
		for(int x = 0; x < terrainSize; x += treeSpacing) 
		{
            for (int z = 0; z < terrainSize; z += treeSpacing) 
			{
				
				float unit = 1.0f / (terrainSize - 1);
				
				float offsetX = Random.value * unit * treeSpacing;
				float offsetZ = Random.value * unit * treeSpacing;
				
                float normX = x * unit + offsetX;
                float normZ = z * unit + offsetZ;
                
                // Get the steepness value at the normalized coordinate.
                float angle = terrain.terrainData.GetSteepness(normX, normZ);
                
				//  Divide by 90 to get an alpha blending value in the range 0..1.
				float frac = angle / 90.0f;
				
				if(frac < 0.5f) //make sure tree are not on steep slopes
				{
					float worldPosX = x+tileX*(terrainSize-1);
					float worldPosZ = z+tileZ*(terrainSize-1);
					
					float noise = treeNoise.FractalNoise(worldPosX, worldPosZ, 3, treeFrq, 1.0f);
					float ht = terrain.terrainData.GetInterpolatedHeight(normX, normZ);
					
					if(noise > 0.0f && ht < terrainHeight*0.4f)
					{
				
						TreeInstance temp = new TreeInstance();
						temp.position = new Vector3(normX,ht,normZ);
						temp.prototypeIndex = Random.Range(0, 3);
						temp.widthScale = 1;
						temp.heightScale = 1;
						temp.color = Color.white;
						temp.lightmapColor = Color.white;
						
						terrain.AddTreeInstance(temp);
					}
				}
				
			}
		}
		
		terrain.treeDistance = treeDistance;
		terrain.treeBillboardDistance = treeBillboardDistance;
		terrain.treeCrossFadeLength = treeCrossFadeLength;
		terrain.treeMaximumFullLODCount = treeMaximumFullLODCount;
		
	}

	/// <summary>
	/// Adds the detail map.
	/// </summary>
	/// <param name="terrain">Terrain.</param>
	/// <param name="tileX">Tile x.</param>
	/// <param name="tileZ">Tile z.</param>
	void AddDetailMap(Terrain terrain, int tileX, int tileZ)
	{
		//each layer is drawn separately so if you have a lot of layers your draw calls will increase 
		int[,] detailMap0 = new int[detailMapSize,detailMapSize];
		int[,] detailMap1 = new int[detailMapSize,detailMapSize];
		int[,] detailMap2 = new int[detailMapSize,detailMapSize];
		
		float ratio = (float)terrainSize/(float)detailMapSize;
		
		Random.seed = 0;
	
		for(int x = 0; x < detailMapSize; x++) 
		{
            for (int z = 0; z < detailMapSize; z++) 
			{
				detailMap0[z,x] = 0;
				detailMap1[z,x] = 0;
				detailMap2[z,x] = 0;
					
				float unit = 1.0f / (detailMapSize - 1);

                float normX = x * unit;
                float normZ = z * unit;
                
                // Get the steepness value at the normalized coordinate.
                float angle = terrain.terrainData.GetSteepness(normX, normZ);
                
                // Steepness is given as an angle, 0..90 degrees. Divide
                // by 90 to get an alpha blending value in the range 0..1.
                float frac = angle / 90.0f;
				
				if(frac < 0.5f)
				{
					float worldPosX = (x+tileX*(detailMapSize-1))*ratio;
					float worldPosZ = (z+tileZ*(detailMapSize-1))*ratio;
					
					float noise = detailNoise.FractalNoise(worldPosX, worldPosZ, 3, detailFrq, 1.0f);
					
					if(noise > 0.0f) 
					{
						float rnd = Random.value;
						//Randomly select what layer to use
						if(rnd < 0.33f)
							detailMap0[z,x] = 1;
						else if(rnd < 0.66f)
							detailMap1[z,x] = 1;
						else
							detailMap2[z,x] = 1;
					}
				}
				
			}
		}
		
		terrain.terrainData.wavingGrassStrength = wavingGrassStrength;
		terrain.terrainData.wavingGrassAmount = wavingGrassAmount;
		terrain.terrainData.wavingGrassSpeed = wavingGrassSpeed;
		terrain.terrainData.wavingGrassTint = wavingGrassTint;
		terrain.detailObjectDensity = detailObjectDensity;
		terrain.detailObjectDistance = detailObjectDistance;
		terrain.terrainData.SetDetailResolution(detailMapSize, detailResolutionPerPatch);
		
		terrain.terrainData.SetDetailLayer(0,0,0,detailMap0);
		terrain.terrainData.SetDetailLayer(0,0,1,detailMap1);
		terrain.terrainData.SetDetailLayer(0,0,2,detailMap2);
		
	}
	
	public bool splatDataCreated(){
		Splat[] splats = GameObject.FindObjectsOfType<Splat> ();
		splatPrototypes = new SplatPrototype[splats.Length];
		for(int i=0; i<splats.Length; i++){
			splatPrototypes[i] = new SplatPrototype();
			splatPrototypes[i].texture = splats[i].splatTexture;
			splatPrototypes[i].tileSize = splats[i].splatTileSize;
		}
		return true;
	}

	public bool treeDataCreated(){
		int noOfTreePrefabs = treePrefabs.Length;
		treePrototypes = new TreePrototype[noOfTreePrefabs];
		for (int i=0; i<noOfTreePrefabs; i++) {
			treePrototypes[i] = new TreePrototype();
			treePrototypes[i].prefab = treePrefabs[i];
		}
		return true;
	}
	public bool detailsDataCreated(){
		Details[] details = GameObject.FindObjectsOfType<Details> ();
		detailPrototypes = new DetailPrototype[details.Length];
		for (int i=0; i<details.Length; i++) {
			detailPrototypes [i] = new DetailPrototype ();
			detailPrototypes [i].prototypeTexture = details[i].detailTexture;
			detailPrototypes [i].renderMode = details[i].renderMode;
			detailPrototypes[i].dryColor = details[i].dryColor;
			detailPrototypes[i].healthyColor = details[i].healthyColor;
		}
		return true;
	}
}


class TerrainPointer{

	Terrain current;
	Terrain left;
	Terrain rigt;
	Terrain top;
	Terrain bottom;
}


