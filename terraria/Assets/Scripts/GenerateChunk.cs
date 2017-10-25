using UnityEngine;
using System.Collections;

public class GenerateChunk : MonoBehaviour
{

    public GameObject DirtBlock;
    public GameObject StoneBlock;
    public GameObject GrassBlock;
    public GameObject treeBottom;
    public GameObject treeBlock;
    public GameObject krosnjaBlock;
    public int width;

    public float heightMultiplier;
    public int heightAddition;

    public float smoothness;

    [HideInInspector]
    public float seed;

    public GameObject blockCoal;
    public GameObject blockGold;
    public GameObject blockIron;

    public float chanceCoal;
    public float chanceGold;
    public float chanceIron;

    void Start()
    {
        //seed = Random.Range(-100000f, 100000f);
        Generate();
    }

    public void Generate()
    {
        bool oreTime = false;
        int[] oreStart = new int[1000];
        int[] oreLine = new int[1000];
        int[] oreWidth = new int[1000];
        for (int i = 0; i < 1000; i++)
        {
            oreLine[i] = -99;
            oreWidth[i] = -99;
            oreStart[i] = -99;
        }

        int stevecGrup = 0;
        for (int i = 0; i < width; i++)
        {
            int h = Mathf.RoundToInt(Mathf.PerlinNoise(seed, (i + transform.position.x) / smoothness) * heightMultiplier) + heightAddition;

            int currentGroup = 0;
            for (int j = 0; j < h; j++)
            {
                GameObject selectedBlock = StoneBlock;
                if (j < h - 7)
                {

                    //da najdemo, katera grupa orov se že spawna
                    for (int k = 0; k < 1000; k++)
                    {
                        if (j == oreStart[k] && oreWidth[k] > 0)
                        {
                            //Debug.Log("ore START na j:" + j + " current group: " + k);
                            oreWidth[k]--;
                            oreTime = true;
                            currentGroup = k;
                            break;
                        }

                    }

                    if (oreTime)
                    {
                        //Debug.Log("ore line:" + oreLine[currentGroup] + " j: " + j + "  (current group: " + currentGroup);
                        if (oreLine[currentGroup] < 1)
                        {
                            oreTime = false;
                            //oreLine[currentGroup] = Random.Range(3, 6);
                            //Debug.Log("ore width:"+oreWidth[currentGroup]);
                            if (oreWidth[currentGroup] > 1)
                                oreLine[currentGroup] = 4;
                            else
                            {
                                oreLine[currentGroup] = 2;
                                oreStart[currentGroup] += 1;
                            }
                                
                        }
                        else
                        {
                            selectedBlock = blockCoal;
                            oreLine[currentGroup] -=1;
                        }
                    }
                    else
                    {
                        //selectedBlock = blockCoal;
                        double tmpChance = chanceCoal;
                        if(j>h/2)
                        {
                            tmpChance += 0.3;
                        }
                        float random = Random.Range(0f, 100f);
                        if (random <= tmpChance)
                        {
                            //selectedBlock = blockCoal;
                            oreTime = true;
                            currentGroup = stevecGrup;

                            oreStart[stevecGrup] = j;
                            oreWidth[stevecGrup] = Random.Range(4, 7);
                            oreLine[stevecGrup] = 2;
                            //Debug.Log("Creating ore." + "ore start: " + oreStart[stevecGrup]+ "  oreWidth: " + oreWidth[stevecGrup] + " oreLine: " + oreLine[stevecGrup] + "(oreGrup:"+stevecGrup);
                            stevecGrup++;
                            
                        }
                    }

                }
                else if (j < h - 1)
                {
                    selectedBlock = DirtBlock;
                }
                else {
                    selectedBlock = GrassBlock;
                }

                GameObject newBlock = Instantiate(selectedBlock, Vector3.zero, Quaternion.identity) as GameObject;
                newBlock.transform.parent = this.gameObject.transform;
                newBlock.transform.localPosition = new Vector3(i, j);
                newBlock.GetComponent<Item>().SetHealthToBlocks();
            }
            //Trees
            int randTree = Random.Range(0, 100);
            if(randTree < 10)
            {
                int treeHeight = Random.Range(4, 7);

                GameObject newBlockBottom = Instantiate(treeBottom, Vector3.zero, Quaternion.identity) as GameObject;
                newBlockBottom.transform.parent = gameObject.transform;
                newBlockBottom.transform.localPosition = new Vector3(i, h);
                newBlockBottom.GetComponent<Item>().SetHealthToBlocks();
                h++;

                for (int l=1;l<treeHeight;l++)
                {
                    GameObject newBlock = Instantiate(treeBlock, Vector3.zero, Quaternion.identity) as GameObject;
                    newBlock.transform.parent = gameObject.transform;
                    newBlock.transform.localPosition = new Vector3(i, h);
                    newBlock.GetComponent<Item>().SetHealthToBlocks();
                    h++;
                }
                for(int l=0;l<3;l++)
                {
                    int tmpI = i-1;
                    for(int p=0; p<3;p++)
                    {
                        GameObject newBlock = Instantiate(krosnjaBlock, Vector3.zero, Quaternion.identity) as GameObject;
                        newBlock.transform.parent = gameObject.transform;
                        newBlock.transform.localPosition = new Vector3(tmpI, h);
                        tmpI++;
                    }
                    h++;
                }
            }

        }
        //GenerateOres();
    }

    public void GenerateOres()
    {
        foreach (GameObject t in GameObject.FindGameObjectsWithTag("BlockStone"))
        {
            float random = Random.Range(0f, 100f);
            GameObject selectedBlock = null;

            if (random <= chanceGold)
            {
                selectedBlock = blockGold;
            }
            else if (random <= chanceIron)
            {
                selectedBlock = blockIron;
            }
            else if (random <= chanceCoal)
            {
                selectedBlock = blockCoal;
            }

            if (selectedBlock != null)
            {
                Instantiate(selectedBlock, t.transform.position, Quaternion.identity);
                Destroy(t);
            }

        }
    }
}
