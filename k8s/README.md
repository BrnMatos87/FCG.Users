# Kubernetes

Os manifests Kubernetes deste serviço são mantidos no repositório
[`FCG.Orchestration`](https://github.com/BrnMatos87/FCG.Orchestration), que é a
fonte única da implantação no AKS.

- [Manifests da Users](https://github.com/BrnMatos87/FCG.Orchestration/tree/main/k8s/users)
- [Kustomize da solução](https://github.com/BrnMatos87/FCG.Orchestration/tree/main)

Não adicione Secrets ou credenciais neste repositório. Alterações de
infraestrutura devem ser feitas no `FCG.Orchestration` e seguir o fluxo
`fase4` para `main`.
