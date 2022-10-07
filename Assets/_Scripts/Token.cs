using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Token : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	private Camera m_camera;
	private Vector3 m_pointerPositionBeforeDrag;
	private Vector3 m_positionBeforeDrag;

	private int[] m_dragSpace;
	private int m_tokenType;

	public void OnBeginDrag(PointerEventData eventData) {
		m_pointerPositionBeforeDrag = m_camera.ScreenToWorldPoint(eventData.position);
		m_positionBeforeDrag = transform.position;
		GetDragSpace();
	}

    public void OnDrag(PointerEventData eventData) {
		Vector3 mouseWorldPosition = m_camera.ScreenToWorldPoint(eventData.position);
		//«агальне зм≥щенн€ курсору (пальц€) щодо точки, зв≥дки почавс€ дрег:
		Vector3 totalDrag = mouseWorldPosition - m_pointerPositionBeforeDrag;
		//¬изначаЇмо, т€гнемо ф≥шку по горизонтал≥ або по вертикал≥:
		if (Mathf.Abs(totalDrag.x) > Mathf.Abs(totalDrag.y)) {
			//ќбмежуЇмо перем≥щенн€ лише порожн≥ми кл≥тинами всередин≥ пол€
			float posX = Mathf.Clamp(mouseWorldPosition.x, m_positionBeforeDrag.x -
			m_dragSpace[1], m_positionBeforeDrag.x + m_dragSpace[0]);
			//перем≥щуЇмо ф≥шку
			transform.position = new Vector3(posX, m_positionBeforeDrag.y,
			transform.position.z);
		} else {
			//ќбмежуЇмо перем≥щенн€ лише порожн≥ми кл≥тинами всередин≥ пол€
			float posY = Mathf.Clamp(mouseWorldPosition.y, m_positionBeforeDrag.y -
			m_dragSpace[3], m_positionBeforeDrag.y + m_dragSpace[2]);
			//перем≥щуЇмо ф≥шку
			transform.position = new Vector3(m_positionBeforeDrag.x, posY,
			transform.position.z);
		}

	}

	public void OnEndDrag(PointerEventData eventData) {
		AlignOnGrid();
		Controller.Instance.TurnDone();
	}

    void Start()
    {
		m_camera = Camera.main;
		AlignOnGrid();
		m_tokenType = Random.Range(0, Controller.Instance.Level.TokenTypes);
		Material myMaterial = gameObject.GetComponent<Renderer>().material;
		myMaterial.SetColor("_Color", Controller.Instance.TokenColors[m_tokenType]);
		Controller.Instance.TokensByTypes[m_tokenType].Add(this);
		transform.SetParent(Controller.Instance.Field.transform);
	}
    void Update()
    {
        
    }
	private void AlignOnGrid() {
		Vector3 alignedPosition = transform.position;
		alignedPosition.x = Mathf.Round(transform.position.x);
		alignedPosition.y = Mathf.Round(transform.position.y);
		transform.position = alignedPosition;
	}
	private void GetDragSpace() {
		int oddEven = 1;
		if (Controller.Instance.Level.FieldSize % 2 != 0) {
			oddEven = 0;
		}
		m_dragSpace = new int[] { 0, 0, 0, 0 };
		int halfField = (Controller.Instance.Level.FieldSize - 1) / 2;
		m_dragSpace[0] = CheckSpace(Vector2.right);
		if (m_dragSpace[0] == -1) {
			m_dragSpace[0] = halfField - (int)transform.position.x + oddEven;
		}
		m_dragSpace[1] = CheckSpace(Vector2.left);
		if (m_dragSpace[1] == -1) {
			m_dragSpace[1] = halfField + (int)transform.position.x;
		}
		m_dragSpace[2] = CheckSpace(Vector2.up);
		if (m_dragSpace[2] == -1) {
			m_dragSpace[2] = halfField - (int)transform.position.y + oddEven;
		}
		m_dragSpace[3] = CheckSpace(Vector2.down);
		if (m_dragSpace[3] == -1) {
			m_dragSpace[3] = halfField + (int)transform.position.y;
		}
	}
	private int CheckSpace(Vector2 direction) {
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction);
		for (int i = 0; i < hits.Length; i++) {
			if (hits[i].collider.gameObject != gameObject) {
				return Mathf.FloorToInt(hits[i].distance);
			}
		}
		return -1;
	}

}
