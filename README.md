Banco KRT - API de Gestão de Limites PIX

📄 Sobre o Projeto
Este repositório contém uma solução completa para um sistema de gestão de limites de transações PIX. A API foi desenvolvida como parte de um desafio técnico, focando na implementação de um sistema seguro, escalável e de fácil manutenção, seguindo as melhores práticas de mercado para aplicações .NET.
O sistema permite que analistas de fraude gerenciem os limites PIX dos clientes e que o sistema de transações valide se uma operação PIX pode ou não ser realizada de forma atômica e segura.

✨ Funcionalidades Principais

👤 Gestão de Clientes:
Cadastro (Create): Permite registrar novos clientes com seus dados de conta e limite PIX inicial.
Busca (Read): Permite consultar os dados de um cliente específico ou de todas as contas associadas a um documento.
Atualização (Update): Permite alterar o limite PIX de uma conta já cadastrada.
Remoção (Delete): Permite remover o registro de limite de um cliente.

💸 Processamento de Transações:
Valida se uma transação PIX está dentro do limite do cliente.
Debita o valor do limite de forma atômica e segura usando operações condicionais do DynamoDB.

🏛️ Arquitetura e Princípios
Este projeto foi construído sobre uma base sólida de princípios de engenharia de software para garantir qualidade e manutenibilidade.
Clean Architecture: A solução é dividida em quatro camadas independentes (Domain, Application, Infrastructure, WebApi), com as dependências apontando para o núcleo do negócio. Isso garante um baixo acoplamento e alta coesão.

SOLID: Todos os cinco princípios foram aplicados:
S (SRP): Cada classe tem uma única responsabilidade.
O (OCP): A arquitetura está aberta para extensão (novos handlers) e fechada para modificação.
L (LSP): As abstrações (IMediator, IClientePixRepository) garantem a substituibilidade.
I (ISP): Interfaces pequenas e focadas no seu contexto.
D (DIP): As camadas de alto nível dependem de abstrações, não de implementações concretas.

Domain-Driven Design (DDD): Foram aplicados os padrões táticos do DDD, como Entidades, Repositórios e Serviços de Aplicação (implementados como Commands e Queries do MediatR).

CQRS com MediatR: A lógica de negócio é separada em Comandos (escrita) e Consultas (leitura), orquestrada pelo MediatR. Isso mantém os controllers extremamente limpos e a lógica de negócio organizada e testável.

🛠️ Tecnologias Utilizadas
Framework: .NET 8 (ASP.NET Core Web API)
Banco de Dados: AWS DynamoDB (NoSQL)
Testes: xUnit (Framework de Teste) e Moq (Biblioteca de Mocking)
Padrões: MediatR (para CQRS), API Versioning
Documentação: Swagger / OpenAPI

📂 Estrutura do Projeto
BancoKRT-API/
├── src/                      # Código-fonte da aplicação
│   ├── BancoKRT.Domain/        # Entidades e lógica de domínio pura
│   ├── BancoKRT.Application/   # DTOs, interfaces, lógica de aplicação (Handlers do MediatR)
│   ├── BancoKRT.Infrastructure/  # Implementação do acesso ao DynamoDB
│   └── BancoKRT.WebApi/        # Controllers, Middleware, ponto de entrada da API
└── tests/                    # Projetos de teste
    └── BancoKRT.Api.Tests/     # Testes unitários para a camada da API

🚀 Como Executar Localmente
Pré-requisitos
.NET 8 SDK
AWS CLI
Credenciais AWS: Um usuário IAM com permissões programáticas para o DynamoDB.

-- Configuração --
Configure suas credenciais AWS no seu terminal (substitua com suas chaves e região):
Generated sh
aws configure
AWS Access Key ID [None]: SEU_ACCESS_KEY
AWS Secret Access Key [None]: SEU_SECRET_KEY
Default region name [None]: sa-east-1
Default output format [None]: json
Sh

Clone o repositório:
git clone https://github.com/mansque404/BancoKRT-API.git
cd BancoKRT-API

Rode a aplicação:
dotnet run --project src/BancoKRT.WebApi/BancoKRT.WebApi.csproj

A API estará rodando. Acesse a documentação interativa do Swagger no seu navegador: https://localhost:<PORTA>/swagger/index.html.

🧪 Como Rodar os Testes
Para executar a suíte de testes unitários, rode o seguinte comando na raiz do projeto:
dotnet test

Endpoints da API (v1)
Método HTTP	Endpoint	Descrição
POST	/api/v1/clientespix	Cria um novo cliente com seu limite PIX.
GET	/api/v1/clientespix/{documento}/{contaId}	Busca um registro de cliente específico.
PATCH	/api/v1/clientespix/{documento}/{contaId}/limite	Atualiza o limite PIX de uma conta.
DELETE	/api/v1/clientespix/{documento}/{contaId}	Remove um registro de cliente.
POST	/api/v1/clientespix/{documento}/{contaId}/processar-transacao	Processa uma transação PIX.
