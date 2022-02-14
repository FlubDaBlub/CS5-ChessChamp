using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour
{
  void OnMouseDown()
  {
      // Destroy the gameObject after clicking on it
      Destroy(gameObject);
  }
}
