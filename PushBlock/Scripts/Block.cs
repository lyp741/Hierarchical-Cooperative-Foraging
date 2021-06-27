using UnityEngine;
using Unity.MLAgents;

public class Block : MonoBehaviour
{
    public GameObject swarm;
    public GameObject ground;
    public GameObject goal;
    public GameObject split;
    public GameObject body;
    public GameObject s_blocks;
    public Bounds areaBounds;
    public Bounds splitBounds;
    //public Scene cur_scene = Scene.Pusher;
    public SceneController SC;
    // public enum Scene
    // {
    //     Pusher,
    //     s_block_random,
    //     Decompose,
    //     Selector
    // }
    public bool is_goaled = false;
    public Vector3 init_pos;


    void OnCollisionEnter(Collision col){
        if (gameObject.CompareTag("l_block")&&col.gameObject.CompareTag("split")){
            SC.L_Goaled(this);
        } 
        if (gameObject.CompareTag("s_block")&&col.gameObject.CompareTag("goal")){
            SC.S_Goaled(this);
        }
        // if (gameObject.CompareTag("l_block")&&col.gameObject.CompareTag("agent")){
        //     SC.L_Touched(this);
        // }
        // if (gameObject.CompareTag("s_block")&&col.gameObject.CompareTag("agent")){
        //     SC.S_Touched(this);
        //}
    }

    // void OnCollisionEnter_Decompose_Pusher(Collision col){
    //     if ((gameObject.CompareTag("l_block") && col.gameObject.CompareTag("split")) || (gameObject.CompareTag("s_block") && col.gameObject.CompareTag("goal"))){
    //         is_goaled = true;
    //         for (int i=0; i<swarm.transform.childCount; i++){
    //             PushAgent agent = swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>();
    //             agent.Block_goaled(this.gameObject);
    //         }
    //         gameObject.SetActive(false);
    //     }
    //     // if ((gameObject.CompareTag("s_block") && col.gameObject.CompareTag("obstacle")))
    //     // {
    //     //     for (int i=0; i<swarm.transform.childCount; i++){
    //     //         PushAgent agent = swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>();
    //     //         agent.obstacle(this.gameObject);
    //     //     }
    //     // }
    // }

    // void OnCollisionEnter_Selector(Collision col){
    //     if ((gameObject.CompareTag("l_block") && col.gameObject.CompareTag("split"))){
    //         body.SetActive(false);
    //         Rigidbody rb = GetComponent<Rigidbody>();
    //         // rb.constraints = RigidbodyConstraints.FreezeAll;
    //         for (int j=0; j<s_blocks.transform.childCount;j++){
    //             GameObject b = s_blocks.transform.GetChild(j).gameObject;
    //             b.SetActive(true);
    //         }
    //         for (int i=0; i<swarm.transform.childCount; i++){
    //             PushAgent agent = swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>();
    //             agent.Block_goaled(this.gameObject);
    //         }
    //     }else if(gameObject.CompareTag("s_block") && col.gameObject.CompareTag("goal") ){
    //         is_goaled = true;
    //         gameObject.SetActive(false);
    //         // if (GetComponent<Rigidbody>()){
    //         //     Destroy(GetComponent<Rigidbody>());
    //         // }
    //         for (int i=0; i<swarm.transform.childCount; i++){
    //             PushAgent agent = swarm.transform.GetChild(i).gameObject.GetComponent<PushAgent>();
    //             agent.Block_goaled(this.gameObject);
    //         }
    //     }
    // }

    // void OnCollisionEnter(Collision col){
    //     switch (cur_scene){
    //         case Scene.Decompose:
    //         case Scene.Pusher:
    //             OnCollisionEnter_Decompose_Pusher(col);
    //             break;
    //         case Scene.s_block_random:
    //         case Scene.Selector:
    //             OnCollisionEnter_Selector(col);
    //             break;
    //     }
    // }

    // public void reset_block(){
    //     is_goaled = false;
    //     switch (cur_scene){
    //         case Scene.Decompose:
    //             if (gameObject.CompareTag("l_block")){
    //                 transform.position = GetPosforbigfoodinmid();
    //                 // Rigidbody rb = GetComponent<Rigidbody>();
    //                 // rb.velocity = Vector3.zero;
    //                 // rb.angularVelocity = Vector3.zero;                                                                                               
    //                     for (int i=0; i<s_blocks.transform.childCount;i++){
    //                     GameObject b = s_blocks.transform.GetChild(i).gameObject;
    //                     b.SetActive(false);
    //                     b.transform.localPosition = b.GetComponent<Block>().init_pos;
    //                     b.GetComponent<Block>().reset_block();
    //                     }
    //                 gameObject.SetActive(true);
    //             }
    //             break;
    //         case Scene.Pusher:
    //             if (gameObject.CompareTag("s_block")){
    //                 transform.position = GetPosforsmallfoodinmid();
    //                 Rigidbody rb = GetComponent<Rigidbody>();
    //                 rb.velocity = Vector3.zero;
    //                 rb.angularVelocity = Vector3.zero;
    //                 gameObject.SetActive(true);
    //             }
    //             if (gameObject.CompareTag("obstacle")){
    //                 transform.position = GetRandomSpawnPos();
    //             }
    //             if (gameObject.CompareTag("team_b")){
    //                 transform.position = GetPosforsmallfoodinmid();
    //                 for (int i=0; i<s_blocks.transform.childCount;i++){
    //                     GameObject b = s_blocks.transform.GetChild(i).gameObject;
    //                     b.transform.localPosition = b.GetComponent<Block>().init_pos;
    //                     b.SetActive(true);
    //                     if (!b.GetComponent<Rigidbody>()){
    //                     b.AddComponent<Rigidbody>();
    //                     Rigidbody rb_block = b.GetComponent<Rigidbody>();
    //                     rb_block.mass = 15;
    //                     rb_block.drag = 10;
    //                     rb_block.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //                     }
    //                     b.GetComponent<Block>().reset_block();
    //                 }
    //             }
    //             break;
    //         case Scene.s_block_random:
    //             if (gameObject.CompareTag("s_block")){
    //                 transform.position = GetPosforsmallfoodinmid();
    //                 Rigidbody rb = GetComponent<Rigidbody>();
    //                 rb.velocity = Vector3.zero;
    //                 rb.angularVelocity = Vector3.zero;
    //                 gameObject.SetActive(true);
    //             }
    //             if (gameObject.CompareTag("obstacle")){
    //                 transform.position = GetRandomSpawnPos();
    //             }
    //             break;
    //         case Scene.Selector:
    //             if (gameObject.CompareTag("l_block")){
    //                 transform.position = GetPosforbigfoodinmid();
    //                 // Rigidbody rb = GetComponent<Rigidbody>();
    //                 // rb.velocity = Vector3.zero;
    //                 // rb.angularVelocity = Vector3.zero;
    //                 body.SetActive(true);
    //                 // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    //                 for (int i=0; i<s_blocks.transform.childCount;i++){
    //                     GameObject b = s_blocks.transform.GetChild(i).gameObject;
    //                     b.SetActive(false);
    //                     b.transform.localPosition = b.GetComponent<Block>().init_pos;
    //                     b.GetComponent<Block>().reset_block();
    //                 }
    //             }
    //             break;
    //     }
    // }
    PushBlockSettings m_PushBlockSettings;
    void Awake()
    {
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();
    }
    public Vector3 GetRandomSpawnPos()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range((-areaBounds.extents.z+20) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.z-20) * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.3f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(5f, 0.1f, 5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
        }
        return randomSpawnPos;
    }
    // public Vector3 GetPosforbigfoodinmid()
    // {
    //     var foundNewSpawnLocation = false;
    //     var randomSpawnPos = Vector3.zero;
    //     int tried = 0;
    //     while (foundNewSpawnLocation == false && tried<30)
    //     {
    //         var randomPosX = Random.Range((-areaBounds.extents.x+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
    //             (areaBounds.extents.x-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);
    //             //  Debug.Log(areaBounds.extents.x);

    //         var randomPosZ = Random.Range((-areaBounds.extents.z+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
    //             (areaBounds.extents.z-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);
    //         randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.8f, randomPosZ);
    //         if (Physics.CheckBox(randomSpawnPos, new Vector3(8f, 0.7f, 8f)) == false)
    //         {
    //             foundNewSpawnLocation = true;
    //         }
    //         tried++;
    //         if (tried==30){
    //             Debug.Log("not found");
    //         }
    //     }
    //     return randomSpawnPos;
    // }
    public Vector3 GetPosforteamfoodinspl()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        int tried = 0;
        var randomPosX = 0f;
        var randomPosZ = 0f;
        while (foundNewSpawnLocation == false && tried<10)
        {
            randomPosX = Random.Range((-splitBounds.extents.x+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (splitBounds.extents.x-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);

            randomPosZ = Random.Range((-splitBounds.extents.z+5) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (splitBounds.extents.z-5) * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = split.transform.position + new Vector3(randomPosX, 0.8f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.35f, 2.5f)) == false)
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

    public Vector3 GetPosforsmallfoodinmid()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        int tried = 0;
        var randomPosX = 0f;
        var randomPosZ = 0f;
        while (foundNewSpawnLocation == false && tried<10)
        {
            randomPosX = Random.Range((-areaBounds.extents.x+10) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.x-10) * m_PushBlockSettings.spawnAreaMarginMultiplier);

            randomPosZ = Random.Range((-areaBounds.extents.z+30) * m_PushBlockSettings.spawnAreaMarginMultiplier,
                (areaBounds.extents.z-30) * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.8f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.35f, 2.5f)) == false)
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

    // void Update(){
    //     switch (cur_scene){
    //         case Scene.Selector:
    //             if (gameObject.CompareTag("l_block")){
    //                 int n_goaled = 0;
    //                 for (int i=0; i<s_blocks.transform.childCount;i++){
    //                     Block block = (Block) (s_blocks.transform.GetChild(i).gameObject.GetComponent<Block>());
    //                     n_goaled += block.is_goaled ? 1 : 0;
    //                 }
    //                 if (n_goaled == s_blocks.transform.childCount){
    //                     is_goaled = true;
    //                 }
    //             }
    //             break;
    //     }
    // }

//     void Start(){
//         // Debug.Log(transform.localPosition);
//         init_pos = transform.localPosition;
//         areaBounds = ground.GetComponent<Collider>().bounds;
//         splitBounds = split.GetComponent<Collider>().bounds;
//         if (gameObject.CompareTag("l_block")){
            
//         }else if (gameObject.CompareTag("s_block")){

//         }
//     }
}