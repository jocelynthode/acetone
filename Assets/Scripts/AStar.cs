using UnityEngine;
using System.Collections;

public class AStar : MonoBehaviour {
	private const int TILE_SIZE = 1;
	private GridItem[][] map;
	private int offsetX;
	private int offsetY;
	private GridItemSorter sorter = new GridItemSorter();
	private ArrayList items = new ArrayList();

	public static AStar instance = null;
	
	void Awake()
	{
		if (instance == null)		
			instance = this;
		else if(instance != this)
			Destroy(gameObject);
		//DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		string[] names = new string[]{"Floor", "Wall", "OuterWall", "Exit"};
		int[] walkable = new int[]{1, 0, 0, 0};
		//get max grid
		int minX = 0;
		int minY = 0;
		int maxX = 0;
		int maxY = 0;
		for (int l = 0; l < names.Length; l++) {
			GameObject[] tiles = GameObject.FindGameObjectsWithTag(names[l]);
			for (int t = 0; t < tiles.Length; t++){
				Vector3 position = tiles[t].transform.position;
				int roundX = Mathf.RoundToInt(position.x);
				int roundY = Mathf.RoundToInt(position.y);
				if (roundX < minX)
					minX = roundX;
				else if (roundX > maxX)
					maxX = roundX;
				if (roundY < minY)
					minY = roundY;
				else if (roundY > maxY)
					maxY = roundY;
			}
		}
		int verticalCellsAmount = (maxY - minY) / TILE_SIZE + 1;
		int horizontalCellsAmount = (maxX - minX) / TILE_SIZE + 1;
		//create map
		map = new GridItem[verticalCellsAmount][];
		for (int y = 0; y < verticalCellsAmount; y++) {
			map[y] = new GridItem[horizontalCellsAmount];
		}
		offsetX = Mathf.Abs (minX) / TILE_SIZE;
		offsetY = Mathf.Abs (minY) / TILE_SIZE;
		for (int l = 0; l < names.Length; l++) {
			GameObject[] tiles = GameObject.FindGameObjectsWithTag(names[l]);
			for (int t = 0; t < tiles.Length; t++){
				Vector3 position = tiles[t].transform.position;
				int roundX = (int)(position.x / TILE_SIZE + offsetX);
				int roundY = (int)(position.y / TILE_SIZE + offsetY);
				map[roundY][roundX] = new GridItem();
				map[roundY][roundX].tile = tiles[t];
				map[roundY][roundX].mode = walkable[l];
			}
		}
		int rows = map.Length;
		for (int y = 0; y < map.Length; y++) {
			int columns = map[y].Length;
			for (int x = 0; x < map[y].Length; x++) {
				GridItem item = map[y][x];
				if (item == null){
					map[y][x] = item = new GridItem();
				}
				items.Add(item);
				item.bottom = (y == rows - 1) ? -1 : items.Count + columns - 1;
				item.top = (y == 0) ? -1 : items.Count - columns - 1;
				item.left = (x == 0) ? -1 : items.Count - 1 - 1;
				item.right = (x == columns - 1) ? -1 : items.Count + 1 - 1;
			}
		}
	}

	public ArrayList calculatePath(Vector3 startPosition, Vector3 endPosition){
		if (map == null || map.Length == 0)
			return null;
		ArrayList result = new ArrayList ();
		ArrayList open = new ArrayList ();
		ArrayList closed = new ArrayList ();
		//reset
		GridItem start = null;
		GridItem end = null;
		for (int y = 0; y < map.Length; y++) {
			for (int x = 0; x < map[y].Length; x++) {
				map[y][x].f = 0;
				map[y][x].g = 0;
				map[y][x].h = 0;
                if (map[y][x].tile.transform.position == startPosition)
					start = map[y][x];
                else if (map[y][x].tile.transform.position == endPosition)
					end = map[y][x];
			}
		}
		//calculate
		start.g = 0;
		start.h = estimateDistance(start, end);
		start.f = start.g + start.h;
		start.parent = null;
		open.Add(start);
		GridItem n;
		int counter = 0;
		while(open.Count > 0 && counter < 4000){
			n = pop(open);
			//success
			if (n == end){
				//construct path
				while (n != null && n != start){
                    result.Insert(0, n.tile.transform.position);
					n = n.parent;
				}
				//result.Insert(0, start.tile);
				break;
			}
			checkSuccessor(n.bottom, n, end, open, closed);
			checkSuccessor(n.left, n, end, open, closed);
			checkSuccessor(n.top, n, end, open, closed);
			checkSuccessor(n.right, n, end, open, closed);
			closed.Add(n);
			counter++;
		}
		return result;
	}

	private class GridItem: System.Object{
		public GameObject tile;
		public int mode = 0;
		public float g = 0;
		public float h = 0;
		public float f = 0;
		public GridItem parent = null;
		public int left = 0;
		public int right = 0;
		public int top = 0;
		public int bottom = 0;
	}
	
	private float estimateDistance(GridItem item1, GridItem item2){
		Vector3 position1 = item1.tile.transform.position;
		Vector3 position2 = item2.tile.transform.position;
		return Mathf.Abs(position2.x - position1.x) + Mathf.Abs(position2.y - position1.y);
	}
	
	private GridItem pop(ArrayList open){
		open.Sort(sorter);
		GridItem item = (GridItem)open [0];
		open.Remove (item);
		return item;

	}

	private class GridItemSorter : IComparer  {
		int IComparer.Compare( System.Object o1, System.Object o2 )  {
			GridItem n1 = (GridItem)o1;
			GridItem n2 = (GridItem)o2;
			if (n1.f < n2.f)
				return -1;
			if (n1.f > n2.f)
				return 1;
			return 0;
		}
	}

	private void checkSuccessor(int index, GridItem n, GridItem end, ArrayList open, ArrayList closed){
		if (index == -1)
			return;
		GridItem n1 = (GridItem)items[index];
		if (n1.mode == 0)
			return;
		float newg = n.g + 1;
		if ((open.Contains(n1) || closed.Contains(n1)) && n1.g <= newg){
			return;
		}
		n1.parent = n;
		n1.g = newg;
		n1.h = estimateDistance(n1, end);
		n1.f = n1.g + n1.h;
		if (closed.Contains(n1))
			closed.Remove(n1);
		if (!open.Contains(n1)){
			if (n1 != end)
				n1.mode = 2;
			open.Add(n1);
		}
	}

	void Update () {

	}

}
