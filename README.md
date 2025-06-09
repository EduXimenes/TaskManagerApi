# Gerenciador de Projetos

## Visão Geral
O Gerenciador de Projetos é uma aplicação desenvolvida em .NET utilizando a arquitetura Clean Architecture. Ele permite aos usuários organizar e monitorar suas tarefas, bem como colaborar com colegas de equipe. A aplicação foi executada no Docker e utiliza Entity Framework Core para acesso a banco de dados com Postgree e AutoMapper para mapeamento de objetos e segue os princípios de arquitetura limpa para manter um código organizado e modular.

## Funcionalidades Principais
Projetos e Tarefas
- **Listar projetos.**
- **Visualizar detalhes e tarefas de um projeto.**
- **Criar, atualizar e remover projetos (com restrição para exclusão de projetos com tarefas ativas).**
- **Listar, visualizar, adicionar, atualizar e remover tarefas.** \
 *Prioridade da tarefa imutável após criação.* \
 *Limite máximo de 20 tarefas por projeto.* 

Histórico e Comentários
- **Registrar automaticamente histórico de alterações em tarefas.**
- **Listar histórico de alterações.**
- **Adicionar e visualizar comentários vinculados às tarefas.**

Usuários e Autorização
- **CRUD de usuários (sem autenticação configurada para desenvolvimento local).**

Relatórios
**Geração de relatórios de desempenho para usuários com papel de gerente.**

Relatório apresenta o desempenho médio de tarefas concluídas nos últimos 30 dias.

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/products/docker-desktop/)
  
## Como Rodar o Projeto
1. Clone este repositório para o seu ambiente local.
   ```bash
   git clone
   ```
2. Abra o projeto em sua IDE preferida ou em um terminal.
3. Para rodar o projeto digite no terminal:
   ```bash
   docker-compose up --build
   ```
4. O Projeto foi configurado para rodar no Swagger para facilitar a visualização:
   http://localhost:7006/swagger/index.html
5. É possível acessar as API's através do uso do Postman com seus respectivos endpoints.

# Fase 2 - Refinamento

### Questionamentos visando o refinamento para futuras implementações ou melhorias.

- A aplicação está considerando os aumentos de número de usuários e projetos/tarefas ?
- Existem preocupações com desempenho ?
- Existem áreas da aplicação que o usuário considera dificil ou confuso de utilizar? Como entradas de dados ou respostas com dados excessivos.
- Os relatórios gerados para gerentes estão atendendo as expectativas?
- Seria interessante incluir mais dados neste relatório ou criar outros tipos de relatórios?

# Fase 3 - Final

### Melhorarias no projeto

- Criar uma documentação abrangente para desenvolvedores, usuários e administradores do sistema, incluindo guias de uso.
- Para melhoria do projeto seria interessante adicionar recursos robustos de monitoramento e logging para acompanhar o desempenho do aplicativo, identificar problemas rapidamente e tomar medidas direcionadas para melhorias.
- Avaliar melhorias futuras na modularização da aplicação, como a separação de contextos ou serviços, caso o sistema cresça em escopo, visando melhor manutenção, performance e escalabilidade.
