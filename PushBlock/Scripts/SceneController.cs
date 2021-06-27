
using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class SceneController : MonoBehaviour
{
    public GameObject swarm;
    public GameObject s_block;
    public GameObject l_block;
    public GameObject ground;
    public GameObject goal;
    public GameObject split;
    public Bounds areaBounds;
    public Bounds splitBounds;
    public GameObject big_foods;
    public GameObject small_foods;
    //public GameObject foods_small;
    public enum Scene
    {   
        Searcher,
        Searcher_S,
        Searcher_L,
        Pusher,
        Decomposer,
        Selector,
        SelectorNoStock,
        CollectiveTransport,
        Pusher_old,
        Decomposer_old,
        Selector_old
    }
    public Scene cur_scene = Scene.Pusher;
    public int n_s = 2;
    public int n_l = 2;
    public int n_l_goaled;
    public int n_s_goaled;
    public Vector3 position;
    public int stock;
    public int n_stock;
    
    
    
    public void Start()
    {   areaBounds = ground.GetComponent<Collider>().bounds;
        n_l_goaled = 0;
        n_s_goaled = 0;
        switch (cur_scene){
            case Scene.Searcher:
            case Scene.Searcher_L:
            case Scene.Searcher_S:
            case Scene.SelectorNoStock:
            case Scene.Selector_old:
            case Scene.CollectiveTransport:
                for(int i=0;i<n_l;i++){
                    Block LB=Instantiate(l_block,GetPosforbigfoodinmid(),Quaternion.identity,big_foods.transform).GetComponent<Block>();
                    LB.SC = this;
                }
                for(int i=0;i<n_s;i++){
                    Block SB=Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                    SB.SC=this;
                }
                break;
            case Scene.Decomposer:
            case Scene.Decomposer_old:
                for(int i=0;i<n_l;i++){
                    Block LB=Instantiate(l_block,GetPosforbigfoodinmid(),Quaternion.identity,big_foods.transform).GetComponent<Block>();
                    LB.SC = this;
                }
                for(int i=0;i<n_s;i++){
                    Block SB=Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                    SB.SC=this;
                }
                // for(int i=0;i<2;i++){
                //     Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity);
                // }
                break;
            case Scene.Pusher:
            case Scene.Pusher_old:
                for(int i=0;i<n_s;i++){
                    Block SB=Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                    SB.SC=this;
                }
                for(int i=0;i<n_l;i++){
                    Block LB=Instantiate(l_block,GetPosforbigfoodinmid(),Quaternion.identity,big_foods.transform).GetComponent<Block>();
                    LB.SC = this;
                }
                break;
            case Scene.Selector:
                stock=n_stock;
                for(int i=0;i<n_l;i++){
                    Block LB=Instantiate(l_block,GetPosforbigfoodinmid(),Quaternion.identity,big_foods.transform).GetComponent<Block>();
                    LB.SC = this;
                }
                for(int i=0;i<n_s;i++){
                    Block SB=Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                    SB.SC=this;
                }
                break;    
        }
        
    }
    public void L_Goaled(Block l_goaled){
        switch (cur_scene){
            case Scene.Decomposer:
            case Scene.Decomposer_old:
                for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.Block_goaled_Decompose(l_goaled.gameObject);
                    }
                Destroy(l_goaled.gameObject);
                n_l_goaled++;
                if (n_l_goaled == n_l){
                    for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.EndEpisode();
                    }
                    //Start();
                }
                break;
            case Scene.Selector:
            case Scene.SelectorNoStock:
            case Scene.Selector_old:
            case Scene.CollectiveTransport:
                for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.Block_goaled_Selector(l_goaled.gameObject);
                    }
                position=ground.transform.position+l_goaled.transform.position;
                Destroy(l_goaled.gameObject);
                // Debug.Log(position);
                
                for(int i=0;i<3;i++){
                   Block SB=Instantiate(s_block,GetPosinbigfood(position),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                   SB.SC=this; 
                }
                break;
        }

    }
    public void S_Goaled(Block s_goaled){
         switch (cur_scene){
            case Scene.Pusher:
            case Scene.Pusher_old:
                for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.Block_goaled_Pusher(s_goaled.gameObject);
                    }
                Destroy(s_goaled.gameObject);
                n_s_goaled++;
                if(n_s_goaled == n_s){
                   for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.EndEpisode();
                    }
                    //Start(); 
                }
                break;
            case Scene.Selector:
                for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.Block_goaled_Selector(s_goaled.gameObject);
                    }
                Destroy(s_goaled.gameObject);
                stock+=500;
                if(stock>10000){
                    stock=10000;
                }
                //Debug.Log(stock);
                break;
            case Scene.SelectorNoStock:
            case Scene.Selector_old:
            case Scene.CollectiveTransport:
                for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        // float R = (10000-agent.GetTimestep())/10;
                        agent.Block_goaled_Selector(s_goaled.gameObject);
                    }
                Destroy(s_goaled.gameObject);
                n_s_goaled++;
                if(n_s_goaled==(3*n_l+n_s)){
                    for (int i=0; i<swarm.transform.childCount; i++){
                        PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                        agent.EndEpisode();
                    }
                    //Start(); 
                }
                break;
         }
    }
    public void L_Touched(GameObject l_touched){
                Destroy(l_touched.gameObject);
                Block LB=Instantiate(l_block,GetPosforbigfoodinmid(),Quaternion.identity,big_foods.transform).GetComponent<Block>();
                LB.SC = this;
    }
    

    public void S_Touched(GameObject s_touched){
                Destroy(s_touched.gameObject);
                Block SB=Instantiate(s_block,GetPosforsmallfoodinmid(),Quaternion.identity,small_foods.transform).GetComponent<Block>();
                SB.SC=this;
    }

    // void Update_Decompose_Pusher(){
    //     int n_goaled = 0;
    //     //int n_goaled_s = 0;
    //     for (int i=0; i<foods.transform.childCount; i++){
    //         Block block = (Block) (foods.transform.GetChild(i).gameObject.GetComponent<Block>());
    //         n_goaled += block.is_goaled ? 1 : 0;
    //     }
    //     // for (int i=0; i<foods_small.transform.childCount; i++){
    //     //     Block block = (Block) (foods_small.transform.GetChild(i).gameObject.GetComponent<Block>());
    //     //     n_goaled_s += block.is_goaled ? 1 : 0;
    //     // }
    //     if (n_goaled == foods.transform.childCount){
    //         // for (int i=0; i<foods.transform.childCount; i++){
    //         //     Block block = (foods.transform.GetChild(i).gameObject.GetComponent<Block>());
    //         //     block.reset_block();
    //         // }
    //         Debug.Log("end episode");
    //         for (int i=0; i<swarm.transform.childCount; i++){
    //             PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
    //             // float R = (10000-agent.GetTimestep())/10;
    //             agent.EndEpisode();
    //         }
    //     }
    // }

    PushBlockSettings m_PushBlockSettings;
    void Awake()
    {
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();
    }

    public Vector3 GetPosforsmallfoodinmid()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        int tried = 0;
        var randomPosX = 0f;
        var randomPosZ = 0f;
        while (foundNewSpawnLocation == false && tried<30)
        {
            randomPosX = Random.Range((-areaBounds.extents.x+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.x-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);

            randomPosZ = Random.Range((-areaBounds.extents.z+30) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.z-30) * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.8f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(3f, 0.35f, 3f)) == false)
            {  
                foundNewSpawnLocation = true;
            }
            tried++;
            
        }
        if (!foundNewSpawnLocation){
            // Debug.Log(randomPosX.ToString() + "  " + randomPosZ.ToString());
            // Debug.Log("Not found!");
        }
        return randomSpawnPos;
    }

    public Vector3 GetPosinbigfood(Vector3 center)
    {   
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        int tried = 0;
        // var randomPosX = 0f;
        // var randomPosZ = 0f;
        while (foundNewSpawnLocation == false && tried<30)
        {
            randomSpawnPos.x = Random.Range((center.x-4),(center.x+4));
            randomSpawnPos.z = Random.Range((center.z-4),(center.z+4));
            randomSpawnPos.y = 0.5f;
            if (Physics.CheckBox(randomSpawnPos, new Vector3(1f, 0.35f, 1f)) == false)
            {
                foundNewSpawnLocation = true;
                //Debug.Log(randomSpawnPos);
            }
            tried++;
        }
        if (!foundNewSpawnLocation){
            // Debug.Log("Not found!");
        }
        return randomSpawnPos;
    }

    public Vector3 GetPosforbigfoodinmid()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        int tried = 0;
        while (foundNewSpawnLocation == false && tried<30)
        {
            var randomPosX = Random.Range((-areaBounds.extents.x+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.x-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);
                //  Debug.Log(areaBounds.extents.x);
                var randomPosZ = Random.Range((-areaBounds.extents.z+30) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.z-30) * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.6f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(4f, 0.35f, 4f)) == false)
            {
                foundNewSpawnLocation = true;
            }
            tried++;
            if (tried==30){
                Debug.Log("not found");
            }
        }
       // Debug.Log(areaBounds.extents.x);
      //  Debug.Log(areaBounds.extents.z);
        return randomSpawnPos;
    }
    public void Update(){
        //stock=stock-1;
        if (stock<0){
            for (int i=0; i<swarm.transform.childCount; i++){
                PushAgent agent = (swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>());
                // float R = (10000-agent.GetTimestep())/10;
                agent.EndEpisode();
                }
        }
    }
    // void FixedUpdate(){
    //     Update_Decompose_Pusher();
    // }
}