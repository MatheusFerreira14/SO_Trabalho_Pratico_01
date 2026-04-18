# Algoritmo do Banqueiro (C#)

## Descrição

Este projeto implementa o Algoritmo do Banqueiro utilizando C# e múltiplas threads, com o objetivo de evitar deadlocks em sistemas concorrentes.

O sistema simula clientes que solicitam e liberam recursos, enquanto o algoritmo garante que o sistema permaneça em um estado seguro.

---

## Tecnologias utilizadas

* C#
* .NET
* Threads (`System.Threading`)
* Lock (controle de concorrência)

---

## Conceitos aplicados

* Concorrência
* Deadlock
* Sincronização
* Algoritmo do Banqueiro

---

## Estrutura

* `Program.cs` → código principal
* `README.md` → documentação

---

## Como executar

### 1. Criar projeto (caso não tenha)

```bash
dotnet new console -n BankerApp
cd BankerApp
```

### 2. Substituir o conteúdo do Program.cs pelo código do projeto

### 3. Executar

```bash
dotnet run 10 5 7
```

---

## Funcionamento

* 5 clientes (threads) são criados
* Cada cliente:

  * solicita recursos aleatórios
  * libera recursos aleatórios
* O sistema:

  * valida segurança antes de conceder recursos
  * evita estados inseguros (deadlock)

---

## Regra principal

Uma solicitação só é aceita se o sistema permanecer em estado seguro após a alocação.

---

## Saída

O programa exibe:

* solicitações
* liberações
* decisões (aceito ou negado)
* estado atual do sistema

---

## Autor

Matheus Henrique Borges Ferreira
