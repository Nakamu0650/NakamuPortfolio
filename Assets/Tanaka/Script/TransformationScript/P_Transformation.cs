using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class P_Transformation : MonoBehaviour
{



    //public Texture Hibiscus;
    //public Texture Hibiscus_1;
    //public Texture Nemophila;
    //public Texture Nemophila_1;
    //public Texture Gentian;
    //public Texture Gentian_1;

    //public Material BodyMaterial;

   




    // Start is called before the first frame update
    void Start()
    {


    }

  
    public void OnTransformHibiscus(InputAction.CallbackContext context)
    {
        //Transformation Hibiscus

        //BodyMaterial.SetTexture("_MainTex", Hibiscus);
        //BodyMaterial.SetTexture("_1st_ShadeMap", Hibiscus_1);
        if (context.performed)
        {
            


        }


    void OnTransformNemophila(InputAction.CallbackContext context)
    {
            //Transformation Nemophila

            //BodyMaterial.SetTexture("_MainTex", Nemophila);
            //BodyMaterial.SetTexture("_1st_ShadeMap", Nemophila_1);

        if (context.performed)
        {



        }
    }

    void OnTransformGentain(InputAction.CallbackContext context)
    {
            //Transformation Gentain

            //BodyMaterial.SetTexture("_MainTex", Gentian);
            //BodyMaterial.SetTexture("_1st_ShadeMap", Gentian_1);

            if (context.performed)
            {



            }
        }

    
   }


    // Update is called once per frame
    void Update()
    {
       

        
    }
}
