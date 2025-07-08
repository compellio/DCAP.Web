
# DCAP.Web
This repository contains a simple file-based implementation of the Registry component of [Compellio](https://compellio.com)'s Gateway. The Registry is exposed via a REST API which is documented and can be explored by visiting the  `swagger` path (e.g.: `localhost:8080/swagger`).

The DCAP.Web (or an instance of Compellio's Gateway Registry) needs to be accessible by the [compellio/dcap-asset-explorer](https://github.com/compellio/dcap-asset-explorer) for a user-friendly visualization of DCAPs.

**Pre-requisite:** The DCAP.Web needs to connect to an Ethereum-compatible blockchain network (either a mainnet or a testnet) with a valid wallet having enough funds, in order to issue DCAPs/TARs. 
- Should you wish to run DCAP.Web on a local environment, there is a Hardhat Node deployment, according to the instructions in the [compellio/dcap-token-smart-contract](https://github.com/compellio/dcap-token-smart-contract) project. 
- You can use coin faucets (e.g.: [Etherlink faucet](https://faucet.etherlink.com/)) in order to obtain funds for usage with a testnet of your choice (e.g.: [Etherlink testnet](https://testnet.explorer.etherlink.com/))

## Deployment and execution
In the following sections, a `docker-compose` and a plain `docker` method are described for the deployment and execution of DCAP.Web

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
docker pull ghcr.io/compellio/dcap-web:latest
docker run -it \
  -p 8080:8080 \
  -v dcap-web-storage:/data \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e DCAP__PrivateKey=0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80 \
  -e DCAP__BlockchainUrl=http://host.docker.internal:8545/ \
  -e DCAP__BlockchainId=31337 \
  -e DCAP__UriPrefix=https://example.com/dcap/ \
  ghcr.io/compellio/dcap-web:latest
```
## Data initialization
In the following sections, a series of payloads are described which need to be uploaded in DCAP.Web, in order to initialize the DCAP asset profiles.

### Root profile asset schema
First, the root profile asset schema needs to be uploaded in DCAP.Web which has as follows:

```json
{
   "@context": {
      "@version": 1.1,
      "foaf": "http://xmlns.com/foaf/0.1/",
      "schema": "http://schema.org/",
      "skos": "http://www.w3.org/2004/02/skos/core#",
      "xsd": "http://www.w3.org/2001/XMLSchema#",
      "rdf": "http://www.w3.org/1999/02/22-rdf-syntax-ns#",
      "organization_key": {
         "@id": "https://gateway.satp.ietf.org/asset_schema_org_key",
         "@context": {
            "public_key": {
               "@id": "https://gateway.satp.ietf.org/asset_schema_pub_key",
               "@type": "schema:string"
            },
            "issued": {
               "@id": "https://gateway.satp.ietf.org/asset_schema_key_issued",
               "@type": "schema:string"
            }
         }
      },
      "qualities": {
         "@id": "https://gateway.satp.ietf.org/asset_schema_qualities"
      }
   },
   "@id": "https://gateway.satp.ietf.org/asset_schema/"
}
```
This can be achieved through the Swagger interface of DCAP.Web or by directly CURLing as follows: 
```bash
curl -X 'POST' \
  'http://localhost:8080/api/v1/TAR' \
  -H 'accept: text/plain' \
  -H 'Content-Type: application/json' \
  -d '{
   "@context": {
      "@version": 1.1,
      "foaf": "http://xmlns.com/foaf/0.1/",
      "schema": "http://schema.org/",
      "skos": "http://www.w3.org/2004/02/skos/core#",
      "xsd": "http://www.w3.org/2001/XMLSchema#",
      "rdf": "http://www.w3.org/1999/02/22-rdf-syntax-ns#",
      "organization_key": {
         "@id": "https://gateway.satp.ietf.org/asset_schema_org_key",
         "@context": {
            "public_key": {
               "@id": "https://gateway.satp.ietf.org/asset_schema_pub_key",
               "@type": "schema:string"
            },
            "issued": {
               "@id": "https://gateway.satp.ietf.org/asset_schema_key_issued",
               "@type": "schema:string"
            }
         }
      },
      "qualities": {
         "@id": "https://gateway.satp.ietf.org/asset_schema_qualities"
      }
   },
   "@id": "https://gateway.satp.ietf.org/asset_schema/"
}'
```
After uploading this asset profile, the DCAP.Web will return a 201 response with a payload similar to this:
```json
{ "id": "urn:tar:eip155.128123:073d7a18417bc1feb1c88a342f7e26e6a62745ec", "receipt": "a781c353-de57-4171-85ca-5afd51ad610f", "data": { "@context": { "@version": 1.1, "foaf": "http://xmlns.com/foaf/0.1/", "schema": "http://schema.org/", "skos": "http://www.w3.org/2004/02/skos/core#", "xsd": "http://www.w3.org/2001/XMLSchema#", "rdf": "http://www.w3.org/1999/02/22-rdf-syntax-ns#", "organization_key": { "@id": "https://gateway.satp.ietf.org/asset_schema_org_key", "@context": { "public_key": { "@id": "https://gateway.satp.ietf.org/asset_schema_pub_key", "@type": "schema:string" }, "issued": { "@id": "https://gateway.satp.ietf.org/asset_schema_key_issued", "@type": "schema:string" } } }, "qualities": { "@id": "https://gateway.satp.ietf.org/asset_schema_qualities" } }, "@id": "https://gateway.satp.ietf.org/asset_schema/" }, "checksum": "0x233C42EFBDDA44718E1594E0819A67D15F6A3319C1EDC2E92D2DBA0241A78050", "version": 1, "_sdHashes": null }
```
The id of the generated TAR (in this example, `urn:tar:eip155.128123:073d7a18417bc1feb1c88a342f7e26e6a62745ec`) will be needed for the uploading of the DCAP profile asset schema.

### DCAP profile asset schema
Second, the DCAP profile profile asset schema needs to be uploaded in DCAP.Web which has as follows:

```json
{
   "@context": [
		  "urn:tar:eip155.128123:073d7a18417bc1feb1c88a342f7e26e6a62745ec",
	  {
      "@version": 1.1,
      "asset_schema": "https://gateway.satp.ietf.org/asset_schema/",
      "dcap": {
         "@id": "https://www.culture.gov.gr/asset_profile/dcap",
         "@context": {
            "rwa": {
               "@id": "https://www.culture.gov.gr/asset_profile/rwa",
               "@context": {
                  "digital_carrier_id": {
                     "@id": "https://www.culture.gov.gr/asset_profile/digital_carrier_id",
                     "@type": "schema:string"
                  },
                  "digital_carrier_type": {
                     "@id": "https://www.culture.gov.gr/asset_profile/digital_carrier_type",
                     "@type": "schema:string"
                  },
                  "rwa_kind": {
                     "@id": "https://www.culture.gov.gr/asset_profile/rwa_kind",
                     "@container": "@language"
                  },
                  "rwa_description": {
                     "@id": "https://www.culture.gov.gr/asset_profile/rwa_description",
                     "@container": "@language"
                  },
                  "rwa_current_storage": {
                     "@id": "https://www.culture.gov.gr/asset_profile/rwa_current_storage",
                     "@container": "@language"
                  },
                  "rwa_storage_location": {
                     "@id": "https://www.culture.gov.gr/asset_profile/rwa_storage_location",
                     "@container": "@language"
                  }
               }
            },
            "dcar": {
               "@id": "https://www.culture.gov.gr/asset_profile/dcar",
               "@context": {
                  "dar_id": {
                     "@id": "https://www.culture.gov.gr/asset_profile/dar_id",
                     "@type": "schema:string"
                  },
                  "dar_system_id": {
                     "@id": "https://www.culture.gov.gr/asset_profile/dar_system_id",
                     "@type": "schema:string"
                  },
                  "dar_url": {
                     "@id": "https://www.culture.gov.gr/asset_profile/dar_url",
                     "@type": "schema:url"
                  },
                  "dar_description": {
                     "@id": "https://www.culture.gov.gr/asset_profile/dar_description",
                     "@container": "@language"
                  }
               }
            }
         }
      }
   }],
   "@id": "https://www.culture.gov.gr/asset_profile",
   "schema:title": "Asset Profile for Cultural Assets",
   "schema:organization": {
      "@id": "https://www.culture.gov.gr/",
      "schema:email": "info@culture.gov.gr",
      "tar:organization_key": {
         "tar:public_key": "did:v1:test:nym:JApJf12r82Pe6PBJ3gJAAwo8F7uDnae6B4ab9EFQ7XXk#authn-key-1",
         "tar:issued": "2018-03-15T00:00:00Z"
      }
   },
   "tar:qualities": {
      "owmner_transferability": "non transferable",
      "network_transferability": "evm_compatible_network",
      "jurisdiction": {
         "ownerJurisdictionScope": {
            "law": "123/xyz",
            "territory": "Greece"
         }
      }
   }
}
```
**Attention:** The urn ID at the 3rd line of the JSON payload is the TAR ID of the **root profile asset schema**, as obtained in the previous payload that has been uploaded.

After uploading this asset profile, the DCAP.Web will return a 201 response with a payload similar to this:

```json
{ "id": "urn:tar:eip155.128123:79db37caec7648cfc77cfa0a38876d2db7aaa6e1", "receipt": "4d28dbce-f863-4605-a617-bf9e0754faae", "data": { "@context": [ "urn:tar:eip155.128123:073d7a18417bc1feb1c88a342f7e26e6a62745ec", { "@version": 1.1, "asset_schema": "https://gateway.satp.ietf.org/asset_schema/", "dcap": { "@id": "https://www.culture.gov.gr/asset_profile/dcap", "@context": { "rwa": { "@id": "https://www.culture.gov.gr/asset_profile/rwa", "@context": { "digital_carrier_id": { "@id": "https://www.culture.gov.gr/asset_profile/digital_carrier_id", "@type": "schema:string" }, "digital_carrier_type": { "@id": "https://www.culture.gov.gr/asset_profile/digital_carrier_type", "@type": "schema:string" }, "rwa_kind": { "@id": "https://www.culture.gov.gr/asset_profile/rwa_kind", "@container": "@language" }, "rwa_description": { "@id": "https://www.culture.gov.gr/asset_profile/rwa_description", "@container": "@language" }, "rwa_current_storage": { "@id": "https://www.culture.gov.gr/asset_profile/rwa_current_storage", "@container": "@language" }, "rwa_storage_location": { "@id": "https://www.culture.gov.gr/asset_profile/rwa_storage_location", "@container": "@language" } } }, "dcar": { "@id": "https://www.culture.gov.gr/asset_profile/dcar", "@context": { "dar_id": { "@id": "https://www.culture.gov.gr/asset_profile/dar_id", "@type": "schema:string" }, "dar_system_id": { "@id": "https://www.culture.gov.gr/asset_profile/dar_system_id", "@type": "schema:string" }, "dar_url": { "@id": "https://www.culture.gov.gr/asset_profile/dar_url", "@type": "schema:url" }, "dar_description": { "@id": "https://www.culture.gov.gr/asset_profile/dar_description", "@container": "@language" } } } } } } ], "@id": "https://www.culture.gov.gr/asset_profile", "schema:title": "Asset Profile for Cultural Assets", "schema:organization": { "@id": "https://www.culture.gov.gr/", "schema:email": "info@culture.gov.gr", "tar:organization_key": { "tar:public_key": "did:v1:test:nym:JApJf12r82Pe6PBJ3gJAAwo8F7uDnae6B4ab9EFQ7XXk#authn-key-1", "tar:issued": "2018-03-15T00:00:00Z" } }, "tar:qualities": { "owmner_transferability": "non transferable", "network_transferability": "evm_compatible_network", "jurisdiction": { "ownerJurisdictionScope": { "law": "123/xyz", "territory": "Greece" } } } }, "checksum": "0x56B360152924C6B45400A7C47A701B8BFBF6B0F3F1A5A534EFE8BF4C1EADBCDA", "version": 1, "_sdHashes": null }
```
The id of the generated TAR (in this example, `urn:tar:eip155.128123:79db37caec7648cfc77cfa0a38876d2db7aaa6e1`) will be needed for the uploading of an example DCAP and for the configuration of the [compellio/dcap-asset-explorer](https://github.com/compellio/dcap-asset-explorer) (i.e.: setting the `NEXT_PUBLIC_DEFAULT_SEARCH_QUERY` as follows:  `NEXT_PUBLIC_DEFAULT_SEARCH_QUERY={"data.@context":"urn:tar:eip155.128123:79db37caec7648cfc77cfa0a38876d2db7aaa6e1"}`.)

### Example DCAP
Finally, an example DCAP will be uploaded in DCAP.Web in order to be visualized with the [compellio/dcap-asset-explorer](https://github.com/compellio/dcap-asset-explorer), which has as follows:

```json
{
    "@context": "urn:tar:eip155.128123:79db37caec7648cfc77cfa0a38876d2db7aaa6e1",
    "@type": [
        "dcap:CulturalHeritageObject",
        "edm:ProvidedCHO"
    ],
    "dc:creator": [
        {
            "@value": "Massier, Clément",
            "@language": "en"
        }
    ],
    "dc:identifier": [
        "1974.106 (Inventarnummer)"
    ],
    "dc:subject": [
        {
            "@value": "Flower Ornaments",
            "@language": "en"
        }
    ],
    "dc:title": {
        "en": "Vase"
    },
    "dc:type": [
        {
            "@value": "Decorative objects",
            "@language": "en"
        },
        {
            "@id": "http://data.europeana.eu/concept/96",
            "@type": "edm:Concept",
            "prefLabel": {
                "en": "Art Nouveau"
            }
        }
    ],
    "dcterms:created": [
        "1888"
    ],
    "dcterms:spatial": [
        {
            "@value": "Hamburg (Standort)",
            "@language": "en"
        },
        {
            "@value": "Vallauris (Golfe Juan (Alpes Maritimes)) (Entwurf)",
            "@language": "en"
        }
    ],
    "edm:type": "IMAGE"
}
```
**Attention:** The urn ID at the 2nd line of the JSON payload is the TAR ID of the **DCAP profile asset schema**, as obtained in the previous payload that has been uploaded.


After uploading this asset profile, the DCAP.Web will return a 201 response with a payload similar to this:

```json
{ "id": "urn:tar:eip155.128123:e52af626d8e4c7b7fbd10e4f4865d6b3470f48ab", "receipt": "5eea38b8-2571-42b5-9340-9c4da4376cae", "data": { "@context": "urn:tar:eip155.128123:79db37caec7648cfc77cfa0a38876d2db7aaa6e1", "@type": [ "dcap:CulturalHeritageObject", "edm:ProvidedCHO" ], "dc:creator": [ { "@value": "Massier, Clément", "@language": "en" } ], "dc:identifier": [ "1974.106 (Inventarnummer)" ], "dc:subject": [ { "@value": "Flower Ornaments", "@language": "en" } ], "dc:title": { "en": "Vase" }, "dc:type": [ { "@value": "Decorative objects", "@language": "en" }, { "@id": "http://data.europeana.eu/concept/96", "@type": "edm:Concept", "prefLabel": { "en": "Art Nouveau" } } ], "dcterms:created": [ "1888" ], "dcterms:spatial": [ { "@value": "Hamburg (Standort)", "@language": "en" }, { "@value": "Vallauris (Golfe Juan (Alpes Maritimes)) (Entwurf)", "@language": "en" } ], "edm:type": "IMAGE" }, "checksum": "0x16994CF0B54ED8E20C71B80EF527DC8C03D7D2754230D2C3F83322FDB13E0D83", "version": 1, "_sdHashes": null }
```
The id of the generated TAR (in this example, `urn:tar:eip155.128123:e52af626d8e4c7b7fbd10e4f4865d6b3470f48ab
`) is the ID of the DCAP that you can later see at the [compellio/dcap-asset-explorer](https://github.com/compellio/dcap-asset-explorer)
