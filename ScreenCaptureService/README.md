# ScreenCaptureService

Servis koji periodički uzima screenshot. Interval (u milisekundama) u kojem se uzimaju screenshotovi određuje se u appsettings.json
fileu. U istom konfiguracijskom fileu postavljaja se i rezolucija ekrana za koji se radi screenhot te podatci o konekciju na bazu
podataka

Zadnji dostupni screenshot šalje se spojenom klijentu preko TCP konekcije (server podržava spajanje samo jednog klijenta). Prilikom
spajanja klijent šalje interval u kojem želi dobivati slike.

Prilikom gašenja servisa u bazu se spremaju podatci o vremenu kad je servis za izradu screenshotova počeo raditi, kada je prestao
te koliko screenshotova je u tom periodu napravljeno.