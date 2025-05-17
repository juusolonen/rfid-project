# Core devicellä ajettava koodi

Täältä löytyy lähdekoodi josta voidaan rakentaa Greengrass deployment. Tehty ajettavaksi Raspberry Pi 4:llä.

## Sisältö

Koostuu kahdesta erillisestä osasta, joita molempia ajetaan omissa Docker konteissaan laitteella. 

* Flask-taustasovellus hallinnoimaan laitetta. Kommunikointi UI:n kanssa HTTP:n ja websocketin välityksellä
* React.js -pohjainen web-käyttöliittymä

SQLite datatiedosto tallentuu /home/ggc_user/greengrass_data kansioon.
Kansio luodaan kun sovellus käynnistetään ensimmäisen kerran Greengrassin kautta.
Muulloin se täytyy luoda itse:

```sh
mkdir -p /home/ggc_user/greengrass_data
chmod 777 /home/ggc_user/greengrass_data
```


## Vaatimukset
### AWS IoT Greengrass v2

Tähän sisältyy mm. AWS CLI, java kehityskitti ja Nucleus

Asennusohje: [here](https://docs.aws.amazon.com/greengrass/v2/developerguide/getting-started.html).



### Docker

   
* [Docker](https://docs.docker.com/engine/install/debian/)
    * [Katso myös](https://docs.docker.com/engine/install/linux-postinstall)
* [docker compose plugin](https://docs.docker.com/compose/install/linux)


### GDK CLI

Tämä auttaa kehityksen aikana
    
[Ohjeet](https://docs.aws.amazon.com/greengrass/v2/developerguide/greengrass-development-kit-cli.html)

### 1.0.0

| Dependency | Compatible versions | Dependency type |
|---|---|---|
| aws.greengrass.Nucleus | >=2.4.0 | Soft |

## Build ja julkaisu

### GDK configuraatiotiedoston päivitys

Päivitä **bucket** ja **region** vastaamaan haluttuja arvoja **gdk-config.json** tiedostoon.



## ⚠️ AJO ILMAN GREENGRASSIA ⚠️

Greengrass-deploymenttia tarvitaan käytettäessä viestinlähetysominaisuutta.
Se kuitenkin aiheuttaa sovelluksen käynnistykseen suuren viiveen, joten muun toiminnallisuuden testaaminen on suotavaa ilman sitä.
Viestin lähetys AWS:n on kommentoitu ulos koodista, mikä täytyy muuttaa jos tarkoitus on saada viestit lähtemään.
Katso: **backend/app/socket_handlers.py** -> handle_message-metodi

**Näin käynnistät ilman Greengrassia:**

```sh
cd ./src
docker compose up --build
```

---


### GDK komennot

```console
gdk component build
```
Tämän jälkeen

```console
sudo /greengrass/v2/bin/greengrass-cli deployment create --recipeDir ~/<polku hakemistoon>/greengrass-build/recipes --artifactDir ~/<polku hakemistoon>/greengrass-build/artifacts --merge "rfid.Web=1.0.0"
```
Niin saat sovelluksen käyntiin laitteella ilman deploymenttia. Tämä on myös syytä tehdä koodimuutosten jälkeen.


```console
sudo /greengrass/v2/bin/greengrass-cli component restart --names "rfid.Web=1.0.0"
```
Käynnistää komponentin uudelleen. Tämäkin on tehtävä koodimuutosten jälkeen, jos komponentti on jo ajossa.



```console
sudo /greengrass/v2/bin/greengrass-cli deployment create --remove="rfid.Web=1.0.0"
```
Poistaa komponentin laitteelta.


Greengrassiin littyviä lokeja voi seurata:

```console
sudo tail -f /greengrass/v2/logs/greengrass.log
```

```console
sudo tail -f /greengrass/v2/logs/rfid.Web.log
```


Seuraava komento julkaisee sovelluksen AWS:n. Ei tarvetta kehityksen aikana.

```console
gdk component publish
```


## Greengrass deployment (Ei tarvitse vielä kehitysvaiheessa)

### Päivitä IAM rooli

AWS IAM rooli tarvitsee oikeudet käyttää S3 buckettia

### AWS IoT Greengrass & Deployments

Buildaa sovellus

```console
gdk component build
```

Julkaise, jolloin paketti siirtyy AWS S3 buckettiin

```console
gdk component publish
```

Lisätietoja: [julkaisu](https://docs.aws.amazon.com/greengrass/v2/developerguide/publish-components.html) sekä [käyttöönotto laitteella](https://docs.aws.amazon.com/greengrass/v2/developerguide/manage-deployments.html)

## Komponentin käyttö

### AWS IoT MQTT test client

AWS IoT MQTT test clientillä voi varmistaa viestin saapumisen AWS:n
Subsribe topiciin **<Thingin_nimi>/publish** ja triggeröi viestin lähetys laitteelta

### Laitteen web-käyttöliittymä

Samassa lähiverkossa olevalla laitteella, siirry selaimella osoitteeseen **http://<laitteen_ip_tai_hostname>:3000** 
