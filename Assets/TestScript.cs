using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {

	void Start () {
        print("Первое включение testSctipt");
	}
	
	void FixedUpdate () {
        print(Time.fixedTime); // Просто выводим время работы движка
	}
}
