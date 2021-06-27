using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class PushAgent : Agent
{
    public GameObject ground;
    public GameObject area;
    public GameObject goal;
    public GameObject split;
    public GameObject smallfoods;
    public GameObject bigfoods;

    //public GameObject obstacles;

    [HideInInspector]
    public Bounds areaBounds;
    PushBlockSettings m_PushBlockSettings;
    public Rigidbody m_AgentRb;
    Material m_GroundMaterial;
    Renderer m_GroundRenderer;
    EnvironmentParameters m_ResetParams;
    Renderer self_renderer;
    public Material pusher_mat;
    public Material decomposer_mat;
    public Material searcher_mat;
    public SceneController SC;
    public bool is_training;
    public bool is_blocked = false;
    public bool is_collide = false;
    public bool collide_l = false;
    public bool collide_s = false;
    public bool Stock;
    GameObject is_pushing_sblock;
    GameObject is_pushing_lblock;
    public int timestep = 0;
    public float[] last_action;
    public Vector3 InitialLocation;
    public Scene cur_scene = Scene.Pusher;
    public enum Scene
    {
        Searcher,
        Pusher,
        Decompose,
        Selector,
        SelectorNoStock,
        CollectiveTransport
        // old_Pusher,
        // old_Decomposer,
        // old_selector
    }
    public PushAgent searcher;
    public PushAgent pusher;
    public PushAgent decomposer;
    public GameObject body_cube;
    public GameObject Reward_UI;
    public GameObject Bord_UI;
    public GameObject stock_UI;
    public bool is_detected;
    bool Switch_On = false;
    //float mindistance = 99f;
    // public GameObject thefood;


    void Awake()
    {
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();
    }

    public override void Initialize()
    {
        if (is_training){
            m_AgentRb = GetComponent<Rigidbody>();
            areaBounds = ground.GetComponent<Collider>().bounds;
            m_GroundRenderer = ground.GetComponent<Renderer>();
            m_GroundMaterial = m_GroundRenderer.material;
            m_ResetParams = Academy.Instance.EnvironmentParameters;
            InitialLocation = transform.position;
            SetResetParameters();
            self_renderer = body_cube.GetComponent<Renderer>();
        }
    }
    

    public Vector3 GetRandomSpawnPos()
    {
        var foundNewSpawnLocation = false;
        var randomSpawnPos = Vector3.zero;
        while (foundNewSpawnLocation == false)
        {
            var randomPosX = Random.Range(-areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.x * m_PushBlockSettings.spawnAreaMarginMultiplier);

            var randomPosZ = Random.Range(-areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier,
                areaBounds.extents.z * m_PushBlockSettings.spawnAreaMarginMultiplier);
            randomSpawnPos = ground.transform.position + new Vector3(randomPosX, 0.3f, randomPosZ);
            if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
            {
                foundNewSpawnLocation = true;
            }
            
        }
        return randomSpawnPos;
    }

    IEnumerator GoalScoredSwapGroundMaterial(Material mat, float time)
    {
        m_GroundRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for 2 sec
        m_GroundRenderer.material = m_GroundMaterial;
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        if (Input.GetKey(KeyCode.D))
        {
            actionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            actionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2;
        }
    }

    public void SetGroundMaterialFriction()
    {
        var groundCollider = ground.GetComponent<Collider>();

        groundCollider.material.dynamicFriction = m_ResetParams.GetWithDefault("dynamic_friction", 0);
        groundCollider.material.staticFriction = m_ResetParams.GetWithDefault("static_friction", 0);
    }

    public void SetBlockProperties()
    {
        var scale = m_ResetParams.GetWithDefault("block_scale", 2);
        //Set the scale of the block
        // m_BlockRb.transform.localScale = new Vector3(scale, 0.75f, scale);

        // // Set the drag of the block
        // m_BlockRb.drag = m_ResetParams.GetWithDefault("block_drag", 0.5f);
    }

    void SetResetParameters()
    {
        SetGroundMaterialFriction();
        SetBlockProperties();
        timestep = 0;
    }

    public void MoveAgent(float[] act)
        {   
            if (is_training && act.Length>0){
                var dirToGo = Vector3.zero;
                var rotateDir = Vector3.zero;

                var action = Mathf.FloorToInt(act[0]);

                switch (action)
                {
                    case 1:
                        dirToGo = transform.forward * 1f;
                        break;
                    case 2:
                        dirToGo = transform.forward * -1f;
                        break;
                    case 3:
                        rotateDir = transform.up * 1f;
                        break;
                    case 4:
                        rotateDir = transform.up * -1f;
                        break;
                    case 5:
                        dirToGo = transform.right * -0.75f;
                        break;
                    case 6:
                        dirToGo = transform.right * 0.75f;
                        break;
                }
                transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
                m_AgentRb.AddForce(dirToGo * m_PushBlockSettings.agentRunSpeed,
                    ForceMode.VelocityChange);
                
            }
            last_action = act;
        }

    public void Select_action(float[] vectorAction){
        var action = Mathf.FloorToInt(vectorAction[0]);
        var child_action = vectorAction;
        // Debug.Log("action: " + action.ToString());
        switch (action)
        {
            case 1:
                child_action = pusher.last_action;
                self_renderer.material = pusher_mat;
                break;
            case 0:
                child_action = decomposer.last_action;
                self_renderer.material = decomposer_mat;
                break;
            case 2:
                child_action = searcher.last_action;
                self_renderer.material = searcher_mat;
                break;
        }
        MoveAgent(child_action);
        //AddReward(-5f / MaxStep);
    }

    public void Select_action_old(float[] vectorAction){
        var action = Mathf.FloorToInt(vectorAction[0]);
        var child_action = vectorAction;
        // Debug.Log("action: " + action.ToString());
        switch (action)
        {
            case 1:
                child_action = pusher.last_action;
                self_renderer.material = pusher_mat;
                break;
            case 0:
                child_action = decomposer.last_action;
                self_renderer.material = decomposer_mat;
                break;
        }
        MoveAgent(child_action);
        //AddReward(-5f / MaxStep);
    }
    
    public void Select_action_Pusher(float[] vectorAction){
        var child_action = searcher.last_action;
        Switch_On = false;
        self_renderer.material = searcher_mat;
        for (int i=0; i<SC.small_foods.transform.childCount; i++){
                Block block = (SC.small_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                float dis = (transform.position-block.transform.position).magnitude;
                if(dis<9){
                    child_action=vectorAction;
                    self_renderer.material = pusher_mat;
                    Switch_On = true;
                    break;
                }
            }
        if(!Switch_On){
            for (int i=0; i<SC.big_foods.transform.childCount; i++){
                Block block = (SC.big_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                float dis = (transform.position-block.transform.position).magnitude;
                if(dis<12){
                    child_action=vectorAction;
                    self_renderer.material = pusher_mat;
                    break;
                }
            }
        }
        
        AddReward(-5f/MaxStep);
        MoveAgent(child_action);
    }

    public void Select_action_Decomposer(float[] vectorAction){
        var child_action = searcher.last_action;
        Switch_On = false;
        self_renderer.material = searcher_mat;
        for (int i=0; i<SC.big_foods.transform.childCount; i++){
                Block block = (SC.big_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                float dis = (transform.position-block.transform.position).magnitude;
                if(dis<13){
                    child_action=vectorAction;
                    self_renderer.material = decomposer_mat;
                    Switch_On = true;
                    break;
                }
            }
        if(!Switch_On){
            for (int i=0; i<SC.small_foods.transform.childCount; i++){
                Block block = (SC.small_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                float dis = (transform.position-block.transform.position).magnitude;
                if(dis<6){
                    child_action=vectorAction;
                    self_renderer.material = decomposer_mat;
                    break;
                }
            }
        }
        AddReward(-5f/MaxStep);
        MoveAgent(child_action);

    }


    public override void OnActionReceived(float[] vectorAction)
    {
        if(is_training){
        switch (SC.cur_scene){
            case SceneController.Scene.Decomposer:
                Select_action_Decomposer(vectorAction);
                timestep=timestep+1;
                break;
            case SceneController.Scene.Pusher:
                Select_action_Pusher(vectorAction);
                timestep=timestep+1;
                break;
            case SceneController.Scene.Searcher:
                MoveAgent(vectorAction);
                timestep = timestep+1;
                // if (is_training && is_collide==false){

                //     //AddReward(-5f / MaxStep);
                //     //float dis = measureTargetDistance();
                //     // float R = dis/1.73f*0.25f;
                //     //float R =0.05f/dis;
                //     //AddReward( -R );
                //     //AddReward( R );
                //     // Debug.Log("dis is " + dis.ToString());
                // }
                for (int i=0; i<SC.small_foods.transform.childCount; i++){
                    Block block = (SC.small_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                    float dis = (transform.position-block.transform.position).magnitude;
                    if(dis<9){
                        SC.S_Touched(block.gameObject);
                        S_Block_touched();
                        break;
                    }
                }
                for (int i=0; i<SC.big_foods.transform.childCount; i++){
                    Block block = (SC.big_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                    float dis = (transform.position-block.transform.position).magnitude;
                    if(dis<13){
                        SC.L_Touched(block.gameObject);
                        L_Block_touched();
                        break;
                    }
                }
                if (is_blocked == true){
                    // SetReward(-10f);
                    Debug.Log("collide obs");
                    //EndEpisode();
                }
                if (is_collide == true){
                     //SetReward(-2f);
                    // Debug.Log("collide agents");
                    //EndEpisode();
                }
                break;
            case SceneController.Scene.Searcher_L:
                MoveAgent(vectorAction);
                timestep = timestep+1;
                for (int i=0; i<SC.big_foods.transform.childCount; i++){
                    Block block = (SC.big_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                    float dis = (transform.position-block.transform.position).magnitude;
                    if(dis<13){
                        SC.L_Touched(block.gameObject);
                        L_Block_touched();
                        break;
                    }
                }
                break;
            case SceneController.Scene.Searcher_S:
                MoveAgent(vectorAction);
                timestep = timestep+1;
                for (int i=0; i<SC.small_foods.transform.childCount; i++){
                    Block block = (SC.small_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                    float dis = (transform.position-block.transform.position).magnitude;
                    if(dis<9){
                        SC.S_Touched(block.gameObject);
                        S_Block_touched();
                        break;
                    }
                }
                break;
            case SceneController.Scene.CollectiveTransport:
            case SceneController.Scene.Pusher_old:
            case SceneController.Scene.Decomposer_old:
                MoveAgent(vectorAction);
                timestep = timestep+1;
                if (is_training && is_collide==false){

                    AddReward(-5f / MaxStep);
                    //float dis = measureTargetDistance();
                    // float R = dis/1.73f*0.25f;
                     //float R =0.05f/dis;
                    //AddReward( -R );
                    // AddReward( R );
                    // Debug.Log("dis is " + dis.ToString());
                }
                if (is_blocked == true){
                    // SetReward(-10f);
                    Debug.Log("collide obs");
                    //EndEpisode();
                }
                if (is_collide == true){
                     SetReward(-2f);
                    // Debug.Log("collide agents");
                    //EndEpisode();
                }
                break;
            case SceneController.Scene.SelectorNoStock:
                Select_action(vectorAction);
                timestep=timestep+1;
                break;
            case SceneController.Scene.Selector_old:
                Select_action_old(vectorAction);
                timestep=timestep+1;
                break;
            case SceneController.Scene.Selector:
                if(Stock){
                    SC.stock = SC.stock-1;
                }
                Select_action(vectorAction);
                timestep = timestep+1;
                AddReward(1f);
                //Debug.Log("Reward");
                // if(thefood.active){
                // float distance = measureTargetDistance();
                //     // float R = dis/1.73f*0.25f;
                //      float r =0.05f/distance;
                //     //AddReward( -R );
                //       //AddReward( r );
                //      }
                break;
        }
        }
        else{
            MoveAgent(vectorAction);
        }
        
    }

    //  public float measureTargetDistance()
    //     {
	//  		Vector3 vec = this.transform.position - thefood.transform.position;
	//  		float mag = vec.magnitude;
	//  		return mag;
	//  	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("l_block") ){
            is_pushing_lblock = col.gameObject;
            collide_l=true;
            switch (SC.cur_scene){
                case SceneController.Scene.Pusher:
                    AddReward(-1f);
                    break;
            }
        }
        if (col.gameObject.CompareTag("s_block")){
            is_pushing_sblock = col.gameObject;
            collide_s=true;
            switch (SC.cur_scene){
                case SceneController.Scene.Decomposer:
                    AddReward(-1f);
                    break;
            }
        }
        // if (col.gameObject.CompareTag("obstacle")){

        //     is_blocked = true;
        // }
        if (col.gameObject.CompareTag("agent")){

            is_collide = true;
        }
    }

    void OnCollisionExit(Collision col){
        if (col.gameObject.CompareTag("l_block")){
            is_pushing_lblock = null;
            collide_l=false;
        }
        if (col.gameObject.CompareTag("s_block")){
            is_pushing_sblock = null;
            collide_s=false;
        }
        // if (col.gameObject.CompareTag("obstacle")){

        //     is_blocked = false;
        //     //    Debug.Log("collide obstacle");
        //     //    SetReward(0f);
        // }
        if (col.gameObject.CompareTag("agent")){

            is_collide  = false;
            //    Debug.Log("collide obstacle");
            //    SetReward(-2f);
        }
    }

    void Observation_Decompose(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);
        if(is_training && is_pushing_lblock){
            Vector3 splittoblock = split.transform.position - is_pushing_lblock.transform.position;
            AddReward(Vector3.Dot(splittoblock.normalized, is_pushing_lblock.GetComponent<Rigidbody>().velocity));
        }
        if(collide_s){
            AddReward(-2f);
        }
    }

    void Observation_Pusher(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);

        if(is_training && is_pushing_sblock){
            Vector3 goaltoblock = goal.transform.position - is_pushing_sblock.transform.position;
            float reward = Vector3.Dot(goaltoblock.normalized, is_pushing_sblock.GetComponent<Rigidbody>().velocity);
            AddReward(reward);
        }
        if(collide_l){
            AddReward(-2f);
        }
    }

    void Observation_Searcher(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);
    }

    void Observation_Selector(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        int stock=SC.stock;
        sensor.AddObservation(stock);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);
        // if(collide_l){
        //     AddReward(+0.02f);
        // }
        //  if(collide_s){
        //     AddReward(+0.02f);
        // }
    }

    void Observation_SelectorNoStock(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);
        if(collide_l){
            AddReward(+0.02f);
        }
         if(collide_s){
            AddReward(+0.02f);
        }
    }

    void Observation_CollectiveTransport(VectorSensor sensor){
        float x = (split.transform.position.x - transform.position.x);
        float z = (split.transform.position.z - transform.position.z);
        float r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        x = (goal.transform.position.x - transform.position.x);
        z = (goal.transform.position.z - transform.position.z);
        r = Mathf.Sqrt(x * x + z * z);
        sensor.AddObservation(x/r);
        sensor.AddObservation(z/r);
        sensor.AddObservation(m_AgentRb.velocity);
        if(is_training && is_pushing_lblock){
            Vector3 splittoblock = split.transform.position - is_pushing_lblock.transform.position;
            AddReward(Vector3.Dot(splittoblock.normalized, is_pushing_lblock.GetComponent<Rigidbody>().velocity));
        }
        else if(is_training && is_pushing_sblock){
            Vector3 goaltoblock = goal.transform.position - is_pushing_sblock.transform.position;
            float reward = Vector3.Dot(goaltoblock.normalized, is_pushing_sblock.GetComponent<Rigidbody>().velocity);
            AddReward(reward);
        }
        // if(collide_s){
        //     AddReward(-2f);
        // }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        switch (cur_scene){
            case Scene.Searcher:
                Observation_Searcher(sensor);
                break;
            case Scene.Decompose:
                Observation_Decompose(sensor);
                break;
            case Scene.Pusher:
                Observation_Pusher(sensor);
                break;
            case Scene.Selector:
                Observation_Selector(sensor);
                break;
            case Scene.SelectorNoStock:
                Observation_SelectorNoStock(sensor);
                break;
            case Scene.CollectiveTransport:
                Observation_CollectiveTransport(sensor);
                break;
        }
        if(is_training){
            TextMesh t = (TextMesh)Reward_UI.GetComponent(typeof(TextMesh));
            TextMesh ts = (TextMesh)Bord_UI.GetComponent(typeof(TextMesh));
            TextMesh tss=(TextMesh)stock_UI.GetComponent(typeof(TextMesh));
            float r = m_Reward;
            if(m_Reward<=0)
            {
                t.color = Color.red;
            }
            else
            {
                t.color = Color.green;
            }
            t.text =r.ToString("0.00");
            ts.text ="Time Step: " + timestep.ToString();
            switch(cur_scene){
                case Scene.Selector:
                    tss.text = "Stock:" + SC.stock.ToString();
                    break;
            }
            
            }
    }

    public void Block_goaled(GameObject block){
        switch (SC.cur_scene){
            case SceneController.Scene.Decomposer:
                Block_goaled_Decompose(block);
                break;
            case SceneController.Scene.Pusher:
                Block_goaled_Pusher(block);
                break;
            case SceneController.Scene.Selector:
                Block_goaled_Selector(block);
                break;
            case SceneController.Scene.CollectiveTransport:
                Block_goaled_CollectiveTransport(block);
                break;
        }
    }

    public void Block_goaled_CollectiveTransport(GameObject block){
        if (is_training){
            if (block == is_pushing_lblock){
                Debug.Log("large food goaled");
                AddReward(100);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_lblock = null;
                collide_l=false;
            }
            else if (block == is_pushing_sblock){
                Debug.Log("small food goaled");
                AddReward(100);
                //AddReward((MaxStep-timestep)/10);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_sblock = null;
                collide_s=false;
            }
        }
    }

    public void L_Block_touched(){
        if (is_training){
            Debug.Log("large food touched");
            AddReward(10);
            StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
            //is_pushing_lblock = null;
            //collide_l=false;
        }
    }

    public void S_Block_touched(){
        if (is_training){
                Debug.Log("small food touched");
                AddReward(10);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                //is_pushing_sblock = null;
                //collide_s=false;
        }
    }

    public void Block_goaled_Decompose(GameObject block){
        if (is_training){
            if (block == is_pushing_lblock){
                Debug.Log("large food goaled");
                AddReward(100);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_lblock = null;
            }
        }
    }

    public void Block_goaled_Pusher(GameObject block){
        if (is_training){
            //Debug.Log(block);
            //Debug.Log(is_pushing_sblock);
            if (block == is_pushing_sblock){
                Debug.Log("small food goaled");
                AddReward(100);
                //AddReward((MaxStep-timestep)/10);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_sblock = null;
            }
        }
    }

    public void Block_goaled_Selector(GameObject block){
        if (is_training){
            if (block == is_pushing_sblock){
                Debug.Log("get reward Small");
                AddReward(5);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_sblock = null;
                collide_s=false;

            }
             if (block == is_pushing_lblock){
                Debug.Log("get big");
                AddReward(100);
                StartCoroutine(GoalScoredSwapGroundMaterial(m_PushBlockSettings.goalScoredMaterial, 0.5f));
                is_pushing_lblock = null;
                collide_l=false;

            }
        }
    }
    // public void obstacle(GameObject block){
    //     if (is_training){
    //         if (block == is_pushing){
    //              Debug.Log("get reward -0.25");
    //              AddReward(-0.25f);
    //         }
    //     }
    // }
    public int GetTimestep(){
        return timestep;
    }
    public override void OnEpisodeBegin(){
        
        if (is_training){
            var randomSpawnPos = Vector3.zero;
            // for (int i=0; i<foods.transform.childCount; i++){
            //     Block block = (foods.transform.GetChild(i).gameObject.GetComponent<Block>());
            //     block.reset_block();
            // }
            // for (int i=0; i<foods_crap.transform.childCount; i++){
            //     Block crap = (foods_crap.transform.GetChild(i).gameObject.GetComponent<Block>());
            //     crap.reset_block();
            // }
            // for (int i=0; i<obstacles.transform.childCount; i++){
            //     Block obs = (obstacles.transform.GetChild(i).gameObject.GetComponent<Block>());
            //     obs.reset_block();
            // }
            randomSpawnPos = InitialLocation;
            
            transform.position = randomSpawnPos;
            m_AgentRb.velocity = Vector3.zero;
            m_AgentRb.angularVelocity = Vector3.zero;
            SetResetParameters();
            for (int i=0; i<SC.big_foods.transform.childCount; i++){
                Block lblock = (SC.big_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                Destroy(lblock.gameObject);
            }
            for (int i=0; i<SC.small_foods.transform.childCount; i++){
                Block sblock = (SC.small_foods.transform.GetChild(i).gameObject.GetComponent<Block>());
                Destroy(sblock.gameObject);
            }
            SC.Start();
        }
    }
}

