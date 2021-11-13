using UnityEngine;
using System.Collections;

public class BurgerPartSpawner : MonoBehaviour
{
    public Vector2 spawnDeltaMinMax;
    public GameObject burgerPartPrefab;
    public Transform burger;

    GameMaster master;

    void Start ()
    {
        master = GameMaster.instance;

        StartCoroutine(SpawnParts());
	}

    IEnumerator SpawnParts()
    {
        int size = master.ingredients.Length + 1;
        var possibleSpawns = new BurgerPartBlueprint[size];

        for(int i = 0; i < size - 1; i++)
        {
            possibleSpawns[i] = master.ingredients[i];
        }

        possibleSpawns[size - 1] = master.top;

        for(int index = 0; !GameMaster.over; index++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(GameMaster.minX, GameMaster.maxX), Burger.instance.col.transform.position.y + 10, 0);

            var clone = Instantiate(burgerPartPrefab, spawnPos, Quaternion.identity, transform);
            var part = clone.GetComponent<BurgerPart>();

            part.Apply(possibleSpawns[Random.Range(0, size)], index);

            yield return new WaitForSeconds(Random.Range(spawnDeltaMinMax.x, spawnDeltaMinMax.y));
        }
    }
}
