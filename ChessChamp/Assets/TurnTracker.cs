using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class TurnTracker : MonoBehaviour {
    SpriteRenderer m_SpriteRenderer;
    Color m_NewColor;

    void start() {
      m_SpriteRenderer = GetComponent<SpriteRenderer>();
      m_SpriteRenderer.color = new Color32(80, 124, 159, 255);
    }

    public void white() {
      m_SpriteRenderer = GetComponent<SpriteRenderer>();
      m_SpriteRenderer.color = new Color32(80, 124, 159, 255);
    }

    public void black() {
      m_SpriteRenderer = GetComponent<SpriteRenderer>();
      m_SpriteRenderer.color = new Color32(210, 95, 64, 255);
    }
}
