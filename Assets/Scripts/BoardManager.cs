using UnityEngine;
using System;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.
using Range = Utils.Range;

public class BoardManager : MonoBehaviour
{

    private int columns = 25;                                         //Number of columns in our game board.
    private int rows = 15;                                            //Number of rows in our game board.


    //Lower and upper limit for our random number of inner walls and items
    private Range wallCount = new Range(20, 20);
    private Range itemAdCount = new Range(0, 1, 8);
    private Range itemBombCount = new Range(0, 1, 4);
    private Range itemViewbotCount = new Range(0, 1, 8);
    public GameObject playerTile;
    public GameObject exitTile;                                     //Prefab to spawn for exit.
    public GameObject itemAdTile;
    public GameObject itemBombTile;
    public GameObject itemViewbotTile;
    public GameObject[] wallTiles;                                  //Array of wall prefabs.
    public GameObject[] floorTiles;                                  //Array of floor prefabs.
    public GameObject[] enemyTiles;                                 //Array of enemy prefabs.
    public GameObject[] outerWallTiles;                             //Array of outer tile prefabs.

    public Player player;
    public List<Enemy> enemies;

    private Transform boardHolder;                                  //A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>();   //A list of possible locations to place tiles.
    private List<Vector3> path = new List<Vector3>();

    //Clears our list gridPositions and prepares it to generate a new board.

    void initalisePath()
    {
        path.Clear();
        List<Vector3> vectors = new List<Vector3>();
        for (int i = 0; i < columns - 1; i++)
            vectors.Add(new Vector3(1, 0, 0f));
        for (int i = 0; i < rows - 1; i++)
            vectors.Add(new Vector3(0, 1, 0f));

        Vector3 origine = new Vector3(0, 0, 0f);
        path.Add(origine);
        while (vectors.Count > 0)
        {
            int i = Random.Range(0, vectors.Count);
            origine = origine + vectors[i];
            vectors.Remove(vectors[i]);
            path.Add(origine);

        }

    }

    void InitialiseList()
    {
        initalisePath();
        //Clear our list gridPositions.
        gridPositions.Clear();
        //Loop through x axis (columns).
        for (int x = 0; x < columns; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 0; y < rows; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                Vector3 temp = new Vector3(x, y, 0f);
                if (!path.Contains(temp))
                    gridPositions.Add(temp);
            }
        }
    }




    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        //Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
        for (int x = -1; x < columns + 1; x++)
        {
            //Loop along y axis, starting from -1 to place floor or outerwall tiles.
            for (int y = -1; y < rows + 1; y++)
            {
                //Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                //Check if we current position is at board edge, if so choose a random outer wall prefab from our array of outer wall tiles.
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
                instance.transform.SetParent(boardHolder);
            }
        }

    }


    //RandomPosition returns a random position from our list gridPositions.
    Vector3 RandomPosition()
    {
        //Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
        int randomIndex = Random.Range(0, gridPositions.Count);

        //Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
        Vector3 randomPosition = gridPositions[randomIndex];

        //Remove the entry at randomIndex from the list so that it can't be re-used.
        gridPositions.RemoveAt(randomIndex);

        //Return the randomly selected Vector3 position.
        return randomPosition;
    }


    //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    List<GameObject> LayoutObjectAtRandom(GameObject tile, Range range)
    {
        return LayoutObjectAtRandom(new GameObject[] { tile }, range);
    }

    List<GameObject> LayoutObjectAtRandom(GameObject[] tileArray, Range range)
    {
        //Choose a random number of objects to instantiate within the minimum and maximum limits
        int objectCount = range.RandomInt();


        //Instantiate objects until the randomly chosen limit objectCount is reached
        List<GameObject> objects = new List<GameObject>();
        for (int i = 0; i < objectCount; i++)
        {
            //Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
            Vector3 randomPosition = RandomPosition();

            //Choose a random tile from tileArray and assign it to tileChoice
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            //Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
            objects.Add(Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject);
        }
        return objects;
    }


    //SetupScene initializes our level and calls the previous functions to lay out the game board
    public void SetupScene(int level)
    {
        //Creates the outer walls and floor.
        BoardSetup();

        //Reset our list of gridpositions.
        InitialiseList();

        //Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
        LayoutObjectAtRandom(wallTiles, wallCount);

        //Instantiate a random number of items based on minimum and maximum, at randomized positions.
        if (GameManager.instance.level > 2)
        {
            LayoutObjectAtRandom(itemAdTile, itemAdCount);
            LayoutObjectAtRandom(itemBombTile, itemBombCount);
            LayoutObjectAtRandom(itemViewbotTile, itemViewbotCount);
        }

        //Determine number of enemies based on current level number, based on a logarithmic progression
        int enemyCount = (int)Mathf.Log(level, 2f);

        //Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
        enemies.Clear();
        var enemiesObjects = LayoutObjectAtRandom(enemyTiles, new Range(enemyCount, enemyCount));
        enemiesObjects.ForEach(obj => {
            enemies.Add(obj.GetComponent<Enemy>());
        });

        //Instantiate the exit tile in the upper right hand corner of our game board
        Instantiate(exitTile, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);

        //Instantiate the player tile in the bottom left hand corner
        //GameObject playerObject = Instantiate(playerTile, new Vector3(0, 0, 0f), Quaternion.identity) as GameObject;
        //player = playerObject.GetComponent<Player>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
}