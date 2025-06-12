# DCAP.Web

## Running

**Pre-requisite:** the service needs to connect to an Ethereum-compatible blockchain network to issue TARs. For local environments, we recommend running a Hardhat Node according to the instructions in the [compellio/dcap-token-smart-contract](https://github.com/compellio/dcap-token-smart-contract) project.

### Docker Compose

Make sure that Docker and Docker Compose are [installed](https://docs.docker.com/engine/install/). 

Download the `docker-compose.yml` file to a local directory, and modify it as needed. 

You must at least define the `DCAP__PrivateKey` environment variable. By default, the application will connect to the local Hardhat network configured under the [compellio/dcap-token-smart-contract](https://github.com/compellio/dcap-token-smart-contract) project. To connect to a custom network, set the `DCAP__BlockchainUrl` and `DCAP__BlockchainId`. To provide a URI prefix for the `dataUri` smart contract method, set the `DCAP__UriPrefix`.

Note: for the container to connect to a network running on the host, use the special DNS name `host.docker.internal` instead of `127.0.0.1` or `localhost`s, read more at <https://docs.docker.com/desktop/features/networking>.

For example, you may want to change the paths to the data directory, e.g.:

```yaml
- './dcap-web-data:/data'
```

You may also want to change the default port that the API will bind to from the default 8080 to something else, e.g. for port 8000:

```yaml
ports:
  - 8000:8080
```

Once configured, create and run the container with the following command:

```shell
docker compose up -d
```

### Docker

Alternatively, to avoid using Docker Compose, you may also launch the service using the following Docker command:

```shell
docker run -it \
  -p 8080:8080 \
  -v dcap-web-storage:/app/storage \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e DCAP__PrivateKey=0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80 \
  -e DCAP__BlockchainUrl=http://127.0.0.1:8545/ \
  -e DCAP__BlockchainId=31337 \
  -e DCAP__UriPrefix=https://example.com/dcap/ \
  ghcr.io/compellio/dcap-web:latest
```
