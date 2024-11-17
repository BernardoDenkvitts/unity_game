using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pista : MonoBehaviour
{
    // 334.9f é a posição Z onde começa a 2° pista
    // que é o tamanho da nossa pista
    public float pistaLength = 334.9f;
    public GameObject[] obstacles;

    public Vector2 numberOfObstacles;
    
    public List<GameObject> newObstacles;
    
    void Start()
    {
        // Gera um número aleatório de obstáculos dentro do intervalo definido
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        Debug.Log($"Número de obstáculos a serem gerados: {newNumberOfObstacles}");
        
        // Instancia o número determinado de obstáculos e os adiciona à lista newObstacles
        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }
        
        PositionateObstacle();
    }

    void PositionateObstacle()
    {
        GameObject obstacle = obstacles[Random.Range(0, obstacles.Length)];
        for (int i = 0; i < newObstacles.Count; i++)
        {
            // Define a posição mínima e máxima para posicionar o obstáculo no eixo Z
            // Serve para distribuir os obstáculos pela pista
            // + 6 evita que objetos nasçam muito próximo um do outro
            float zMinPosition = ((pistaLength / newObstacles.Count) * 2) * i + 6;
            float zMaxPosition = zMinPosition + 1;
            
            Vector3 newPosition = new Vector3(0, 0, Random.Range(zMinPosition, zMaxPosition));
            
            // Define a nova posição do obstáculo no espaço local
            newObstacles[i].transform.localPosition = newPosition;
            
            // Ativa o obstáculo
            newObstacles[i].SetActive(true);
            
            Debug.Log($"Obstáculo {i} posicionado em {newPosition}");
        }
    }

    // Funcao para reposicionar a pista
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = new Vector3(0, 0, transform.position.z + 334.9f * 2);
            Invoke("PositionateObstacle", 4f);
        }
    }
    
}
