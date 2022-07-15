private var center : Transform;
private var distance = - 0.5;

private var y = 0.0;

function Start () {
    
    var angles = transform.eulerAngles;
    y = transform.localPosition.y;
    
    center = transform.parent;
                     
}

function LateUpdate () {
         
        //Disable the effect if we are too far from the heat particles
        //Camera culling mask must be set to not render layer 10
        if(Vector3.Distance(transform.position,GetComponent.<Camera>().main.transform.position)<8)
        	gameObject.layer = 0;
        else 
        	gameObject.layer = 2;
 		
 		//Face the transform to the camera and move slightly ahead of the fire particle  		       
        var rotation = Quaternion.LookRotation(GetComponent.<Camera>().main.transform.position - transform.position);
        var position = rotation * Vector3(0.0, y, -distance) + center.position;
        
        transform.rotation = Quaternion (0,rotation.y,0,rotation.w);
        transform.position = position;           
    
}
