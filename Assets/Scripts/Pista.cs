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
    public GameObject coin;
    public Vector2 numOfCoins;
    public List<GameObject> newObstacles;
    public List<GameObject> newCoins;
    
    void Start()
    {
        // Gera um número aleatório de obstáculos dentro do intervalo definido
        int newNumberOfObstacles = (int)Random.Range(numberOfObstacles.x, numberOfObstacles.y);
        int newNumberOfCoins = (int)Random.Range(numOfCoins.x, numOfCoins.y);
        
        // Instancia o número determinado de obstáculos e os adiciona à lista newObstacles
        for (int i = 0; i < newNumberOfObstacles; i++)
        {
            newObstacles.Add(Instantiate(obstacles[Random.Range(0, obstacles.Length)], transform));
            newObstacles[i].SetActive(false);
        }

        for (int i = 0; i < newNumberOfCoins; i++)
        {
            newCoins.Add(Instantiate(coin, transform));
            newCoins[i].SetActive(false);
        }
        
        PositionateObstacle();
        PositionateCoins();
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

            if (i == 0)
            {
                zMinPosition += 10; // Adiciona 10 unidades extras para o primeiro obstáculo
            }

            float zMaxPosition = zMinPosition + 1;
            
            Vector3 newPosition = new Vector3(0, 0, Random.Range(zMinPosition, zMaxPosition));
            
            // Define a nova posição do obstáculo no espaço local
            newObstacles[i].transform.localPosition = newPosition;
            
            // Ativa o obstáculo
            newObstacles[i].SetActive(true);
            
            // Apenas o lixo pode aparecer nas 3 lanes diferentes
            if (newObstacles[i].GetComponent<ChangeLane>() != null)
            {
                // Faz o objeto aparecer em uma das 3 lanes
                newObstacles[i].GetComponent<ChangeLane>().PositionLane();
            } 
            
            Debug.Log($"Obstáculo {i} posicionado em {newPosition}");
        }
    }

    void PositionateCoins()
    {
        // Evita que alguma moeda spawne onde o player começa
        float minZPosition = 10f;
        for (int i = 0; i < newCoins.Count; i++)
        {
            float maxZPosition = minZPosition + 5f;
            float randomZPos = Random.Range(minZPosition, maxZPosition);
            newCoins[i].transform.localPosition = new Vector3(transform.position.x, transform.position.y, randomZPos);
            newCoins[i].SetActive(true);
            // Faz o objeto aparecer em uma das 3 lanes
            newCoins[i].GetComponent<ChangeLane>().PositionLane();
            
            // Distancia das moedas vai ser de pelo menos de 8 a Z da moeda anterior
            minZPosition = randomZPos + 8;
        }
    }

    // Funcao para reposicionar a pista
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().IncreaseSpeed();
            transform.position = new Vector3(0, 0, transform.position.z + 297 * 2);
            PositionateObstacle();
            PositionateCoins();
        }
    }
    
}
