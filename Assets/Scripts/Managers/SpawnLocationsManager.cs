using System.Collections.Generic;
using UnityEngine;

namespace TableMemory
{
    public class SpawnLocationsManager : MonoBehaviour
    {
        [Header("Study settings")]
        public GameObject spawnPlane = null;
        [SerializeField] int numSpawnCols = 10;
        [SerializeField] int numSpawnRows = 1;
        [SerializeField] float spawnBorder = 0.05f;
        [SerializeField] bool randomiseWithinTiles = false;

        private List<GameObject> spawnedObjects = new List<GameObject>();

        public GameObject referenceTile;

        private List<GameObject> tiles = new List<GameObject>();

        private float colSize;
        private float rowSize;

        [Header("Test settings")]
        public Transform testProbeLocation;
        public Transform testInteractionLocation;

        private void Start()
        {
        }
        private void Awake()
        {
            referenceTile.SetActive(false);
        }

        public void Reset()
        {
            foreach (GameObject obj in spawnedObjects)
            {
                Destroy(obj);
            }
            spawnedObjects.Clear();

            foreach (GameObject t in tiles)
            {
                t.SetActive(false);
            }
        }

        public void CreateTileMap()
        {
            gameObject.SetActive(true);
            spawnPlane.SetActive(true);

            Vector3 planeSize = spawnPlane.GetComponent<Collider>().bounds.size;
            Vector3 planePosition = spawnPlane.transform.position;
            Vector3 planeCorner = planePosition - planeSize / 2;

            colSize = planeSize.x / numSpawnCols;
            rowSize = planeSize.z / numSpawnRows;

            float innerColOffset;
            float innerRowOffset;
            float spawnXPos;
            float spawnYPos;
            float spawnZPos;
            Vector3 spawnPosition;

            List<GameObject> newTiles = new List<GameObject>();
            for (int col = 0; col < numSpawnCols; col++)
            {
                for (int row = 0; row < numSpawnRows; row++) 
                {
                    innerColOffset = colSize / 2;
                    innerRowOffset = rowSize / 2;
                    
                    // Spawn location in world co-ordinates
                    // Bottom of grid is lower left corner of spawn plane
                    spawnXPos = planeCorner.x + (col * colSize) + innerColOffset;
                    spawnZPos = planeCorner.z + (row * rowSize) + innerRowOffset;
                    spawnYPos = planeCorner.y;

                    spawnPosition = new Vector3(spawnXPos, spawnYPos, spawnZPos);

                    GameObject newObject = Instantiate(referenceTile, spawnPosition, referenceTile.transform.rotation);

                    float zScale = Mathf.Max(rowSize, colSize);

                    newObject.transform.localScale = new Vector3(colSize, rowSize, zScale);
                    newObject.transform.SetParent(this.transform);
                    newObject.SetActive(false);

                    newTiles.Add(newObject);
                }
            }

            Debug.Log("Tile map created");
            tiles = newTiles;
        }

        public List<TileController> CreateStudyObjects(List<GameObject> objects, string objectTag="")
        {
            float innerColOffset;
            float innerRowOffset;
            float spawnXPos;
            float spawnYPos;
            float spawnZPos;
            Vector3 spawnPosition;

            int randomTileNumberIndex;
            int randomTileNumber;
            GameObject randomTile;
            TileController randomTileController;

            List<TileController> chosenTiles = new List<TileController>();

            List<int> tilesCopy = new List<int>();
            for (int i = 0; i < tiles.Count; i++)
            {
                tilesCopy.Add(i);
            }
            Debug.Log(tilesCopy.Count);
            foreach (GameObject obj in objects)
            {
                // Get random tile index from list of unused tiles
                randomTileNumberIndex = Random.Range(0, tilesCopy.Count);
                randomTileNumber = tilesCopy[randomTileNumberIndex];
                randomTile = tiles[randomTileNumber];
                randomTileController = randomTile.GetComponent<TileController>();

                // Remove tile so it can't have 2 items on it
                tilesCopy.Remove(randomTileNumber);
                
                if (randomiseWithinTiles)
                {
                    // Random position within tile, accounting for border space
                    innerColOffset = Random.Range(-(colSize/2 - spawnBorder), colSize/2 - spawnBorder);
                    innerRowOffset = Random.Range(-(colSize/2 - spawnBorder), rowSize/2 - spawnBorder);
                }
                else
                {
                    innerColOffset = 0.0f;
                    innerRowOffset = 0.0f;
                }

                // Spawn location in world co-ordinates, based on chosen tile
                spawnXPos = randomTile.transform.position.x + innerColOffset;
                spawnZPos = randomTile.transform.position.z + innerRowOffset;
                spawnYPos = randomTile.transform.position.y;

                spawnPosition = new Vector3(spawnXPos, spawnYPos, spawnZPos);

                GameObject newObject = Spawn(obj.gameObject, spawnPosition, objectTag);
                newObject.transform.SetParent(this.transform);

                spawnedObjects.Add( newObject );
                randomTileController.SetObject( newObject );

                chosenTiles.Add(randomTileController);

                // Make tile visible
                //randomTile.SetActive(true);
            }

            return chosenTiles;
        }

        public ObjectController CreateTestObject(ObjectController testStimulus)
        {
            GameObject testGameObject = Spawn(testStimulus.gameObject, testProbeLocation.position, "Interactable");
            ObjectController controller = testGameObject.GetComponent<ObjectController>();

            //controller.CenterPosition(testObjectLocation.position);

            return controller;
        }

        public void EnableTestInteraction(ObjectController testObject)
        {
            testObject.transform.position = new Vector3(testObject.transform.position.x, testInteractionLocation.transform.position.y, testInteractionLocation.transform.position.z);
        }

        private GameObject Spawn(GameObject referenceObject, Vector3 spawnLocation, string objectTag = "")
        {
            Quaternion spawnRotation = referenceObject.transform.rotation;

            GameObject newObject = Instantiate(referenceObject, spawnLocation, spawnRotation);

            if (objectTag.Length > 0) newObject.tag = objectTag;

            newObject.SetActive(true);
            newObject.name = referenceObject.name;

            return newObject;
        }
    }
}


