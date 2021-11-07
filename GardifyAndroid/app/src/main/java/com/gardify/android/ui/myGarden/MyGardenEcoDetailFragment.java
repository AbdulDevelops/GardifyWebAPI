package com.gardify.android.ui.myGarden;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.webkit.WebView;
import android.widget.ProgressBar;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.lifecycle.ViewModelProviders;

import com.gardify.android.R;
import com.gardify.android.utils.UiUtils;

import static com.gardify.android.utils.UiUtils.setupToolbar;

public class MyGardenEcoDetailFragment extends Fragment {


    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        WebView webViewEcoDetail;
        ProgressBar progressBar;
        //setup Toolbar
        setupToolbar(getActivity(), "MEIN GARTEN", R.drawable.gardify_app_icon_mein_garten_normal, R.color.toolbar_all_greyishTurquoise, true);

        View root = inflater.inflate(R.layout.fragment_my_garden_eco_detail, container, false);

        webViewEcoDetail = root.findViewById(R.id.web_view_my_garden_eco_detail);
        progressBar = root.findViewById(R.id.progressBar_my_garden_eco_detail);

        Bundle bundle = getArguments();
        String detailFragName = bundle.getString("ECO_DETAIL");
        String html_string = getRawHtml(detailFragName);
        UiUtils.AppWebViewClients webViewClients = new UiUtils.AppWebViewClients(progressBar);
        webViewClients.shouldOverrideUrlLoading(webViewEcoDetail, html_string);
        webViewEcoDetail.setWebViewClient(webViewClients);
        webViewEcoDetail.loadUrl(html_string);

        return root;
    }

    private String getRawHtml(String detailFragName) {
        String html_string = "";
        switch (detailFragName) {
            case "Komposthaufen":
                html_string = "<html><body><div _ngcontent-ljw-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-ljw-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Der Komposthaufen</h2> <p _ngcontent-ljw-c102=\"\"> Der stille, fleißige Gartenhelfer, der Küchen- und Gartenreste aufnimmt und in wertvollen Humus verwandelt, ist das Symbol für Nachhaltigkeit und Kreislaufwirtschaft im Garten.&nbsp;Damit die Mikroorganismen im Komposthaufen die Metamorphose vom Abfallprodukt zur Nährstoffquelle vollführen können, benötigen sie neben dem richtigen Füllmaterial auch Sauerstoff, Wasser und eine angenehme Temperatur. Um optimale Bedingungen zu schaffen, beachtet folgendes:<br _ngcontent-ljw-c102=\"\"><br _ngcontent-ljw-c102=\"\"><span _ngcontent-ljw-c102=\"\" class=\"t-italic\">Die richtige Standortwahl:</span> Euer Komposthaufen sollte auf natürlichem Untergrund (kein Asphalt oder Beton) und im Schatten oder Halbschatten stehen, sowie mit der Schubkarre gut erreichbar sein. Stellt ihn nicht zu nah an eurem Haus auf. Als Wind- und Sichtschutz dienen Hecken, Sträucher oder Bäume. Geschlossene Wände hingegen eignen sich nicht, da sie die Luftzufuhr zum Komposthaufen verhindern.<br _ngcontent-ljw-c102=\"\"><br _ngcontent-ljw-c102=\"\"><span _ngcontent-ljw-c102=\"\" class=\"t-italic\">Der richtige Behälter:</span> Es gibt verschiedene geeignete Behälter. Allen ist gemein, dass sie wetterfest sind und den Luft- und Wasseraustausch ermöglichen. Wir stellen hier die drei gängigsten Varianten vor. </p> <ul _ngcontent-ljw-c102=\"\"> <li _ngcontent-ljw-c102=\"\"><span _ngcontent-ljw-c102=\"\" class=\"t-italic\">Der Klassiker</span>:Ein Komposter aus Holzlatten. Benutzt eine witterungsbeständige Holzsorte, wie z.B. Lärche. Ihr braucht ca. 20 Latten (Länge 1 m, Breite 10 cm, Dicke 4 cm) und 4 Kanthölzer (5 x 5 cm) als Eckpfeiler. Eventuell müssen einzelne Latten mit der Zeit ausgetauscht werden, daher sollten sie mit einem Stecksystem verbunden und nicht verschraubt werden. Diverse Bauanleitungen findet ihr im Netz. </li> <li _ngcontent-ljw-c102=\"\"><span _ngcontent-ljw-c102=\"\" class=\"t-italic\">Der Drahtige</span>: Ein Drahtgitter-Kompost funktioniert wie ein Lattenkompost, hält aber noch länger.</li> <li _ngcontent-ljw-c102=\"\"><span _ngcontent-ljw-c102=\"\" class=\"t-italic\">Miet me</span>: Ebenfalls beliebt ist die Methode der Kompost-Miete. Dabei wird ein Komposthaufen fertig angelegt und mit einer Plane abgedeckt. Er arbeitet dann eine Weile vor sich hin bevor er umgeschichtet wird. </li> </ul> <p _ngcontent-ljw-c102=\"\"> Im Gegensatz zur Kompost-Miete sollte der Komposthaufen bei den anderen Varianten regelmäßig umgeschichtet werden. Daher lohnt es sich, wenn möglich, zwei Kompostier-Behältnisse gleichzeitig zu besitzen. </p> <h2 _ngcontent-ljw-c102=\"\" class=\"t-13 t-italic\">Das richtige Füllmaterial</h2><br _ngcontent-ljw-c102=\"\"> <div _ngcontent-ljw-c102=\"\" class=\"table-responsive\"> <table _ngcontent-ljw-c102=\"\" class=\"table table-striped\"> <thead _ngcontent-ljw-c102=\"\"> <tr _ngcontent-ljw-c102=\"\"> <th _ngcontent-ljw-c102=\"\" scope=\"col\">Das darf hinein:</th> <th _ngcontent-ljw-c102=\"\" scope=\"col\">In geringen Maßen:</th> <th _ngcontent-ljw-c102=\"\" scope=\"col\">Das darf nicht hinein:</th> </tr> </thead> <tbody _ngcontent-ljw-c102=\"\"> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Gemüse- und Obstreste und Schalen</td> <td _ngcontent-ljw-c102=\"\">Zeitungspapier und Pappe (unbeschichtet, nicht farbig bedruckt)</td> <td _ngcontent-ljw-c102=\"\">Gekochtes oder zubereitetes Essen, besonders Fleisch und andere proteinhaltige Lebensmittel</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Eierschalen</td> <td _ngcontent-ljw-c102=\"\">Federn und Haare</td> <td _ngcontent-ljw-c102=\"\">Von Pilz befallene Pflanzenreste</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Kaffeesatz</td> <td _ngcontent-ljw-c102=\"\">Wildkräuter (ohne Samen)</td> <td _ngcontent-ljw-c102=\"\">behandeltes Holz / beschichtetes Papier</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Getrockneter Rasenschnitt</td> <td _ngcontent-ljw-c102=\"\">Frischer Rasenschnitt</td> <td _ngcontent-ljw-c102=\"\">mineralische Abfälle</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Laub (vorsicht bei Kastanien-, Walnuss- und Eichenlaub)</td> <td _ngcontent-ljw-c102=\"\">Holzstreu, Sägespäne (nur ganz geringe Mengen)</td> <td _ngcontent-ljw-c102=\"\">Holzasche: Bäume nehmen Schadstoffe auf, die beim Verbrennen in der Asche zurückbleibt</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Alte Erde</td> <td _ngcontent-ljw-c102=\"\">Algen (aus dem Gartenteich)</td> <td _ngcontent-ljw-c102=\"\">Pflanzen mit Schädlingsbefall</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Gründschnitt von Stauden und Gehölzen</td> <td _ngcontent-ljw-c102=\"\">Zitrusfrüchte</td> <td _ngcontent-ljw-c102=\"\">Metall, Leder</td> </tr> <tr _ngcontent-ljw-c102=\"\"> <td _ngcontent-ljw-c102=\"\">Blumen-, Balkon- und Zimmerpflanzen</td> <td _ngcontent-ljw-c102=\"\"></td> <td _ngcontent-ljw-c102=\"\">Zitrusfruchtschalten (sind oft gespritzt)</td> </tr> </tbody> </table> </div> <p _ngcontent-ljw-c102=\"\"> Achtet außerdem darauf, dass das eingebrachte Material feucht aber nicht nass ist und es großflächig aufgebracht wird. Eine Diversität im Material lohnt ebenfalls: Berücksichtigt das Kohlenstoff-Stickstoff- Verhältnis, sprich das von holzigem und frischem Material. Grobes Material liegt unten, feineres Material wird gemischt darauf verteilt. Eine Abdeckplane fördert die Wärmeentwicklung (das ist vor allem im Winter wegen der Kälte wichtig) und verhindert gleichzeitig Feuchtigkeitsverlust. Zur Abdeckung eignen sich Stroh- oder Schilfmatten, sowie atmungsaktives Kompostschutzvlies. Folien sollten nur kurzfristig und bei starkem Regen zum Einsatz kommen: Sie verhindern, dass Nährstoffe hinausgespült werden, sind aber gleichzeitig luftdicht. Durch die fehlende Sauerstoffzufuhr, können die Abfälle zu faulen beginnen. </p> <h2 _ngcontent-ljw-c102=\"\" class=\"t-13 t-italic\">Gut zu wissen.</h2> <p _ngcontent-ljw-c102=\"\"> Das „torf“ doch nicht wahr sein: Auch wenn Torf aus Hochmooren wegen seines Säuregrads und seiner Fähigkeit Wasser gut zu binden, gerne benutzt wird, schadet sein Einsatz der Umwelt – und zwar gleich doppelt. Sein Abbau zerstört die jahrhundert- bis jahrtausendalten Moore und mit ihm den Lebensraum zahlreicher Pflanzen und Tiere. Außerdem schadet der Torfabbau dem Klima, denn bei der Entwässerung der Moore entweicht CO2. Es ist deshalb besonders nachhaltig, wenn Gärtner möglichst viel mit ihrer selbst hergestellten Komposterde arbeiten und auf torfhaltige Blumenerde möglichst verzichten.<br _ngcontent-ljw-c102=\"\"><br _ngcontent-ljw-c102=\"\"> Umfassende Informationen findet ihr außerdem in der Kompostfibel des Umweltbundesamt:<br _ngcontent-ljw-c102=\"\"><br _ngcontent-ljw-c102=\"\"><a _ngcontent-ljw-c102=\"\" href=\"https://www.umweltbundesamt.de/sites/default/files/medien/376/publikationen/151207_stg_uba_kompostfibel_web.pdf\" rel=\"noopener noreferrer\" target=\"_blank\" style=\"word-wrap: break-word;\"> https://www.umweltbundesamt.de/sites/default/files/medien/376/publikationen/151207_stg_uba_kompostfibel_web.pdf</a> </p> <div _ngcontent-ljw-c102=\"\" class=\"row\"> <div _ngcontent-ljw-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-ljw-c102=\"\" src=\"https://gardify.de/assets/images/kompost1.jpg\" style=\"width: 100%;\"></div><br> <div _ngcontent-ljw-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-ljw-c102=\"\" src=\"https://gardify.de/assets/images/kompost2.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Vogelbrutkasten":
                html_string = "<html><body><div _ngcontent-kxr-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-kxr-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Der Vogelbrutkasten</h2> <p _ngcontent-kxr-c102=\"\"> Ihr habt bereits Vogelfutterstationen für die gefiederten Freunde im Angebot? Prima! Dann möchten wir euch nun dafür begeistern, euren zwitschernden Gästen auch einen Brutkasten anzubieten. Nistkästen sollten spätestens im März katzensicher (mind. 2-3 m hoch) aufgehängt werden. Wenn man sie den Winter über hängen lässt oder bereits im Herbst anbringt, können sie von Vögeln, Eichhörnchen, Siebenschläfern und Insekten als Schlafplatz in kalten Winternächten genutzt werden.<br _ngcontent-kxr-c102=\"\"><br _ngcontent-kxr-c102=\"\"> Die am häufigsten anzutreffenden Nutzer der Brutkästen sind Kohl- und Blaumeisen, sowie Haus- und Feldsperlinge. Größere Kästen werden gerne von Staren bewohnt. Entscheidend darüber, welcher Vogel sich wohlfühlt, ist die Größe des Kastens und des Einfluglochs: Je kleiner der Vogel, desto kleiner sollte das Einflugloch sein. Vogelarten wie Rotkehlchen und Zaunkönig hingegen bevorzugen eine halboffene Vorderwand an ihrem Brutkasten, eine sogenannte Halbhöhle. Diese sollte vor Katzen und Mardern geschützt an Hauswänden, Schuppen und Gartenhäuschen angebracht werden. Für seltenere Arten (wie Waldkauz oder Mauersegler) gibt es Spezialnistkästen.<br _ngcontent-kxr-c102=\"\"><br _ngcontent-kxr-c102=\"\"></p> <h2 _ngcontent-kxr-c102=\"\" class=\"t-13 t-italic\">Das 1x1 des Brutkastens</h2> <p _ngcontent-kxr-c102=\"\"> Im Internet findet man detaillierte Bauanleitungen (zum Beispiel auf der Homepage des NABU), mit denen das Selberbauen der Nistkästen gelingt. Das ist relativ unkompliziert und manchmal sinnvoller als einen (minderwertigen) Brutkasten zu kaufen. Der NABU empfiehlt, egal ob beim Bau oder Kauf, darauf zu achten, dass selbst die kleinsten Kästen eine Bodenfläche von mindestens 12 x 12 cm messen. Das Einflugloch sollte passend zur Kastengröße gewählt werden (kleinerer Kasten -&gt; kleineres Einflugloch). Es sollte sich im oberen Teil der Vorderseite befinden und knapp zwei Hand breit vom Boden entfernt sein, damit die Jungen im Kasten vor Räubern geschützt sind. Bohrt man kleine Löcher à 5 mm in den Boden, begünstigt dies die Belüftung des Brutkastens. Eine Sitzstange am Flugloch ist überflüssig und erleichtert womöglich Räubern das Klettern am Kasten. </p> <h2 _ngcontent-kxr-c102=\"\" class=\"t-13 t-italic\">Das richtige Material</h2> <p _ngcontent-kxr-c102=\"\"> Zu bevorzugen ist raues, unbehandeltes Naturholz. Eichen-, Robinien- und Lärchenholz eignen sich. Diese Sorten sind besonders haltbar, atmungsaktiv und sorgen für ein gutes Klima im Kasten. Um das Holz vor Feuchtigkeit zu schützen, kann es mit Leinöl eingepinselt werden. Verzichtet auf Brutkästen aus Plastik, diese heizen sich stark auf. Verwendet kein behandeltes oder lackiertes Holz. </p> <h2 _ngcontent-kxr-c102=\"\" class=\"t-13 t-italic\">Hang in there</h2> <p _ngcontent-kxr-c102=\"\"> Der Brutkasten sollte nicht in der prallen Sonne aufgehängt werden und das Einflugloch idealerweise nach Osten oder Südosten ausgerichtet sein. Nicht in dicht verzweigte Bäume, sondern mit freiem Anflug aufhängen. Außerdem sollte der Kasten eher leicht nach vorne, aber niemals nach hinten geneigt, hängen, sonst könnte es hineinregnen. Achtet darauf, dass der Kasten für Räuber nicht erreichbar ist. </p> <h2 _ngcontent-kxr-c102=\"\" class=\"t-13 t-italic\">Den Brutkasten reinigen</h2> <p _ngcontent-kxr-c102=\"\"> Ihr solltet den Brutkasten nach der Brutsaison reinigen. Das schafft Platz für ein neues Nest und vertreibt Flöhe, Milben &amp; Co. Tragt bei der Reinigung Handschuhe und entsorgt das Nistmaterial draußen. In der Regel reicht es, das alte Nest zu entfernen, wenn nötig, kann der Kasten auch ausgebürstet werden. Verzichtet auf Insektensprays und chemische Putzmittel. </p> <div _ngcontent-kxr-c102=\"\" class=\"row\"> <div _ngcontent-kxr-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-kxr-c102=\"\" src=\"https://gardify.de/assets/images/Vogelbrutkasten1.jpg\" style=\"width: 100%;\"></div> <br> <div _ngcontent-kxr-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-kxr-c102=\"\" src=\"https://gardify.de/assets/images/Vogelbrutkasten2.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Totholzhaufen":
                html_string = "<html><body><div _ngcontent-lru-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-lru-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Der Totholzhaufen</h2> <p _ngcontent-lru-c102=\"\"> Bei Gartenarbeiten fällt immer viel Holzschnitt an – bitte werft ihn nicht weg! Denn Nachhaltigkeit, Artenvielfalt und Naturschutz sind aktuelle Themen der Gartengestaltung. Ein Totholzhaufen dient als Unterschlupf, Lebensraum und Nahrungsquelle für verschiedene Tiere und eignet sich gleichzeitig als Gestaltungselement oder Hecke. Das ist ein simpler aber effektiver Beitrag zu einem nachhaltigen Kreislauf im eigenen Garten. </p> <h2 _ngcontent-lru-c102=\"\" class=\"t-13 t-italic\">Die Bewohner</h2> <p _ngcontent-lru-c102=\"\"> Von der Sonne beschienenes Totholz bildet vor allem für wärmeliebende Tiere, wie Wildbienen und Eidechsen Lebensraum. Insekten, Schmetterlinge und Spinnen finden Unterschlupf und Nahrung in den Gehölzen. Amphibien fühlen sich in einem feuchten Ambiente wohl. In den kalten Monaten überwintern Igel und Siebenschläfer gerne im Schutz des Geästes am Boden. Zaunkönig und Rotkehlen nutzen die Gebilde aus Totholz als Nistmöglichkeit. Die Liste der Tiere, denen ihr mit einem Totholzhaufen helfen könnt, ist lang.<br _ngcontent-lru-c102=\"\"> Generell gilt, dass ihr beim Anlegen des Totholzhaufens wenig falsch machen könnt. Schichtet nach Lust und Laune Äste und Zweige, Rindenstücke, Wurzeln und Laub zu einem lockeren Haufen auf und schafft dabei möglichst viele Hohlräume, denn schließlich sollen hier die unterschiedlichsten Tiere einziehen. Es empfiehlt sich außerdem vorab unter dem Totholzhaufen eine Grube auszuheben und mit groben Aststücken zu befüllen. Totholz wird seit Jahrhunderten als Gestaltungselement eingesetzt und besitzt romantischen Charme. </p> <h2 _ngcontent-lru-c102=\"\" class=\"t-13 t-italic\">Strukturschaffend</h2> <p _ngcontent-lru-c102=\"\"> Es kann ganz praktisch als Abgrenzung zum Komposthaufen (dieser benötigt eine luftdurchlässige Umrandung) oder zum Gemüsebeet genutzt werden. Auch ein Gartenteich in der Nähe ist als erweiternder Lebensraum denkbar. Mit den passenden Begleitpflanzen wie Farne, Gräser oder Kletterpflanzen wirkt der Totholzhaufen umso attraktiver. </p> <h2 _ngcontent-lru-c102=\"\" class=\"t-13 t-italic\">Ab durch die Hecke</h2> <p _ngcontent-lru-c102=\"\"> Die Benjeshecke (benannt nach den Landschaftsgärtnern Hermann und Heinrich Benjes) ist eine pflegeleichte und nachhaltige Alternative für eine pflegeintensive Hecke oder teuren Sichtschutz. Im Prinzip entspricht die Hecke einem Totholzhaufen, der von Pfosten gestützt in Form einer Hecke angelegt ist. Dadurch entstehen ein stabiler Wall und ein artenreicher Lebensraum für diverse Tiere. Die Holzpfeiler werden in zwei Reihen in den Boden gesetzt, dazwischen Totholz, Reisig und sonstige Gartenreste aufgeschichtet. Mit der Zeit tragen Vögel Samen ein und aus dem Totholz entsteht neues Leben: Die Hecke beginnt zu blühen. Im Frühjahr und Herbst kann neues Schnittgut nachgelegt werden. Der ungeduldige Gärtner kann seine Hecke auch nach eigenen Vorstellungen bepflanzen. </p> <h2 _ngcontent-lru-c102=\"\" class=\"t-13 t-italic\">Darauf bitte achten</h2> <p _ngcontent-lru-c102=\"\"> Achtet darauf auf Hartholz zurückzugreifen. Optimal sind langsam wachsende Obsthölzer, Buche, Eiche und ähnliches. Vermeidet Baumschnitt von dominanten Pflanzen, wie die Brombeere, die aus dem Schnitt heraus austreiben. Wenn ihr die Hecke auf einem sehr nährstoffreichen Boden anlegt, können sich Hochstauden wie Goldruten und Brennnessel ansiedeln, die durch ihr starkes Wachstum anderen Pflanzen Licht und Raum zum Wachsen nehmen. Haltet in einem solchen Fall die Stauden kurz. Reicht euer Holzschnitt nicht aus, lohnt ein Anruf bei großen Gartenbaubetrieben oder der örtlichen Grünabfallverwertung. Habt dann besonderes Augenmerk darauf, dass das Holz nicht von Krankheiten oder Schädlingen befallen ist. </p> <div _ngcontent-lru-c102=\"\" class=\"row\"> <div _ngcontent-lru-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-lru-c102=\"\" src=\"https://gardify.de/assets/images/totholz.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Trockenmauer":
                html_string = "<html><body><div _ngcontent-eub-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-eub-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Die Trockenmauer</h2> <p _ngcontent-eub-c102=\"\"> Eine Trockenmauer aus Naturstein ist leicht anzulegen und ein optisch ansprechendes Gestaltungselement. Sie wird als Stützmauer am Hang gebaut, fasst Hochbeete ein oder dient als natürlicher Raumteiler im Garten. Neben ihrer reinen Funktion als Mauer bietet sie Lebensraum für Pflanzen und viele (wärmeliebende) Tiere.<br _ngcontent-eub-c102=\"\"><br _ngcontent-eub-c102=\"\"> Bei einer Trockenmauer werden die Natursteine wie der Name schon sagt trocken, ohne mit Mörtel verbunden zu werden, aufeinandergelegt. Dabei entstehen zwischen den Steinen Lücken und Fugen, die lediglich zur Stabilität mit etwas Kies und Sand gefüllt werden. Natürlich sind die Fugen auch beabsichtigt: Sie können bepflanzt werden z.B. mit Mauerpfeffer, Zypressen-Wolfsmilch oder Frühlings-Fingerkraut. Insekten wie Wildbienen finden Unterschlupf, Eidechsen, Kröten und Blindschleichen fühlen sich ebenfalls wohl in den warmen, trockenen Mauerritzen. Sie bieten ihnen sowohl Unterschlupf als auch Nahrung. Für den Bau eurer Trockenmauer eignen sich alle Steine, die in der Natur vorkommen. Besonders beliebt sind Gneis, Granit, Sand- und Kalkstein. Wer seinen Garten ökologisch anlegen möchte, sollte darauf achten, Steine aus der Region zu verwenden. Diese habe einen deutlich kürzeren Transportweg. </p> <h2 _ngcontent-eub-c102=\"\" class=\"t-13 t-italic\">Die Technik</h2> <p _ngcontent-eub-c102=\"\"> Genaue Schritt-für-Schritt-Anleitungen und Video-Tutorials zum Anlegen einer Trockenmauer findet ihr im Internet. Grundsätzlich sollte Folgendes beachtet werden:<br _ngcontent-eub-c102=\"\"> Wählt optimaler Weise einen Platz mit sonniger Südlage und steckt zunächst den Mauerbereich ab. Nicht zuletzt damit die Mauer gerade wird. Eine höhere und schwerere Mauer benötigt ein Fundament, sonst sinken die Steine sehr schnell im Boden ein. Das gefährdet die Stabilität. Grabt einen rund 40 cm tiefen Graben aus und verdichtet ihn mit einem Handstampfer. Zunächst wird der Graben 30 cm mit Schotter aufgefüllt, dann folgt eine Sandschicht, sodass etwa 5 cm unter der Bodenoberfläche ein ebenes Schotterbett entsteht. Durch die Schotterschicht bleibt die Mauer trocken und Sickerwasser zieht nicht zwischen die Steine. Die erste Schicht sollte dann aus großen Steinen mit einer ebenen und breiten Auflagefläche bestehen. Dann werden die Steine nach und nach aufgestapelt. Vermeidet Fugen, die sich waage- und senkrecht kreuzen.<br _ngcontent-eub-c102=\"\"></p> <div _ngcontent-eub-c102=\"\" style=\"border-color: #7a9d34; border-style: solid;\"> <p _ngcontent-eub-c102=\"\" style=\"padding: 10px;\"> Bitte beachtet: Soll eure Trockenmauer höher als 2 Meter werden, braucht ihr einen Standsicherheitsnachweis von einem geprüften Statiker. Das gilt auch, wenn ihr eine Fachfirma mit der Errichtung der Mauer beauftragt. </p> </div> <div _ngcontent-eub-c102=\"\" class=\"row\"> <div _ngcontent-eub-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-eub-c102=\"\" src=\"https://gardify.de//assets/images/Trockenmauer.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Gartenteich":
                html_string = "<html><body><div _ngcontent-xrj-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-xrj-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Der Gartenteich</h2> <p _ngcontent-xrj-c102=\"\"> Wasser übt auf den Menschen seit jeher eine unvergleichliche Faszination aus.&nbsp;Doch ein Gartenteich ist nicht nur schön anzuschauen, auch aus ökologischer Sicht ist er sehr wertvoll: Ein Gartenteich ohne Fische bietet Lebensraum für im Wasser lebende Insekten und Insektenlarven, etwa die der Libelle. Auch entwickelt sich das ökologische Gleichgewicht im Teich so leichter, denn Fische fressen zu viele Insekten und Laich, auch scheiden sie zu viel aus. Außerdem kann ein Teich Vögeln und Igeln als Wasserquelle in trockenen Zeiten dienen. Auch Bienen tummeln sich gerne an Wasserstellen.<br _ngcontent-xrj-c102=\"\"><br _ngcontent-xrj-c102=\"\"> Wasser im Garten verbessert und stabilisiert zusätzlich das lokale Mikroklima. In heißen Wetterperioden kühlt sich über Wasserflächen die Luft durch Verdunstung ab. Natürlich bietet das feuchte Biotop auch Lebensraum für Wasserpflanzen und Amphibien, wie Frösche oder Lurche.<br _ngcontent-xrj-c102=\"\"><br _ngcontent-xrj-c102=\"\"> Hier noch ein paar allgemeine Hinweise, die beim Anlegen eines Teichs zu beachten sind:<br _ngcontent-xrj-c102=\"\"> Auf den richtigen Standort kommt es an. Damit euer zukünftiges Feuchtbiotop ein Erfolg wird, solltet ihr darauf achten, dass das Grundstück an der Stelle eben und sonnig ist, zwei bis drei Schattenstunden am Tag sind optimal. Auch die Umgebung ist wichtig: Der Teich sollte nicht direkt unter einem Baum stehen, sonst kämpft ihr mit Falllaub. Der Teich sollte nicht ringsum von Steinen umgeben sein. Eine Feuchtwiese am Teich dient als Sumpfzone, die durch Regen bedingtes übergelaufenes Wasser aufnehmen kann.<br _ngcontent-xrj-c102=\"\"><br _ngcontent-xrj-c102=\"\"> Ihr solltet den Teich außerdem in unterschiedlich tiefe Zonen einteilen: In der Mitte sollte er bis zu einem Meter tief sein, dort gedeihen Seerosen. Dann kommt eine Zone von 20 bis 50 cm. Die dritte Zone bildet den Übergang zum Ufer: Sie sollte sanft ansteigen, sonst wird sich dort keine Erde halten. </p> <div _ngcontent-xrj-c102=\"\" class=\"row\"> <div _ngcontent-xrj-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-xrj-c102=\"\" src=\"https://gardify.de/assets/images/gartenteich1.jpg\" style=\"width: 100%;\"></div> <div _ngcontent-xrj-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-xrj-c102=\"\" src=\"https://gardify.de/assets/images/gartenteich2.jpg\" style=\"width: 100%;\"></div> </div> </div> </div></body></html>";
                break;
            case "Insektenhotel":
                html_string = "<html><body><div _ngcontent-wdd-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-wdd-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Das Insektenhotel</h2> <p _ngcontent-wdd-c102=\"\"> Im Frühjahr, wenn die Sonne langsam wärmer scheint, kann man im heimischen Garten einen summenden Gast begrüßen: Die Wildbiene. Doch das Summen wird leiser, denn sie ist vom Aussterben bedroht. Mit einem Insektenhotel könnt ihr aktiv zum Fortbestand der Wildbiene beitragen. Anders als die staatenbildende Honigbiene, lebt die Vielzahl der Wildbienen solitär, das heißt sie ist alleine für den Nestbau und die Brutpflege verantwortlich. Die Wildbiene ist für die Bestäubung von Pflanzen sogar wichtiger als die Honigbiene.<br _ngcontent-wdd-c102=\"\"><br _ngcontent-wdd-c102=\"\"> In ihrem einjährigen Lebenszyklus hat die Wildbiene im Frühjahr nur einen kurzen Zeitraum, in dem sie sich paaren und ein Nest bauen kann. Noch ein Grund mehr ihr mit einem Insektenhotel die Suche nach geeigneten Brutkammern zu erleichtern.<br _ngcontent-wdd-c102=\"\"><br _ngcontent-wdd-c102=\"\"> Insektenhotels kann man im Fachhandel kaufen oder selbst bauen. Dazu gibt es zahlreiche Video- Anleitungen im Internet. Manchmal sind die Insektenhotels, die man im Baumarkt, Gartencenter und Discounter findet, mehr Accessoire als funktionsfähig. Sind beispielweise die Löcher nicht sauber gebohrt, wird die Biene sie nicht annehmen, da sie ihre Flügel daran verletzen könnte.<br _ngcontent-wdd-c102=\"\"><br _ngcontent-wdd-c102=\"\"> Grundsätzlich kann man zwischen zwei Arten von Hotels unterscheiden: Nebeneinander angeordnete Holzröhrchen oder in einen Balken gebohrte Löcher. Darauf sollte immer geachtet werden: </p> <h2 _ngcontent-wdd-c102=\"\" class=\"t-13 t-italic\">Das richtige Material</h2> <ul _ngcontent-wdd-c102=\"\"> <li _ngcontent-wdd-c102=\"\">Gut durchgetrocknetes Hartholz verwenden (zum Beispiel Eiche, Esche, Buche oder Obstholz)</li> <li _ngcontent-wdd-c102=\"\">Hohle Schilf- und Bambusstängel</li> <li _ngcontent-wdd-c102=\"\">Natürliche, unbehandelte Materialien verwenden (kein lackiertes Holz)</li> </ul> <h2 _ngcontent-wdd-c102=\"\" class=\"t-13 t-italic\">Die richtige Verarbeitung</h2> <ul _ngcontent-wdd-c102=\"\"> <li _ngcontent-wdd-c102=\"\">Die Hohlräume müssen tief genug sein (10-15 cm)</li> <li _ngcontent-wdd-c102=\"\">Variierende Durchmesser der Öffnungen (5-10 mm)</li> <li _ngcontent-wdd-c102=\"\">Werden ausgehöhlte Stängel benutzt: Auf gleicher Höhe abschließen lassen, sonst können Vögel sie herausziehen</li> <li _ngcontent-wdd-c102=\"\">Wird ein Holzbalken (zum Beispiel Eiche) verwendet: Darauf achten, die Löcher nicht längs zur Holzfaser zu bohren, sondern&nbsp;quer. Sonst platzt das Holz auf!</li> <li _ngcontent-wdd-c102=\"\">Löcher sauber bohren, ohne ausgefransten Rand. Eventuell Holzspan-Reste entfernen</li> </ul> <h2 _ngcontent-wdd-c102=\"\" class=\"t-13 t-italic\">Außerdem</h2> <ul _ngcontent-wdd-c102=\"\"> <li _ngcontent-wdd-c102=\"\">Drahtgitter zum Schutz vor Vögeln mit einem Abstand von mindestens 5 cm anbringen</li> <li _ngcontent-wdd-c102=\"\"><span _ngcontent-wdd-c102=\"\" class=\"t-bold\">Wichtig:</span>&nbsp;Zum Nestbauen brauchen die Bienen Lehm. Ist kein lehmiger Boden vorhanden, kann man den Bienen eine Schale Lehm hinstellen</li> <li _ngcontent-wdd-c102=\"\">Ausreichendes Nahrungsangebot (= Pflanzen) in der Umgebung</li> </ul> <p _ngcontent-wdd-c102=\"\"> Auch interessant: Ein Großteil der nestbauenden Wildbienen nistet nicht in der Wand sondern im sandigen Boden. Ihr könnt ihnen mit einer offenen Sandstelle im Garten helfen. </p> <div _ngcontent-wdd-c102=\"\" class=\"row\"> <div _ngcontent-wdd-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-wdd-c102=\"\" src=\"https://gardify.de//assets/images/insektenhotel1.jpg\" style=\"width: 100%;\"></div> <div _ngcontent-wdd-c102=\"\" class=\"col-md-6 mt-4 text-center\"><img _ngcontent-wdd-c102=\"\" src=\"https://gardify.de//assets/images/insektenhotel2.jpg\" style=\"width: 100%;\"></div> <div _ngcontent-wdd-c102=\"\" class=\"col-md-6 mt-4 text-center\"><br><img _ngcontent-wdd-c102=\"\" src=\"https://gardify.de//assets/images/insektenhotel3.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Fledermauskasten":
                html_string = "<html><body><div _ngcontent-gqh-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-gqh-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Der Fledermauskasten</h2> <p _ngcontent-gqh-c102=\"\"> Die Fledermaus, das geheimnisvolle Wesen der Nacht. Im Frühling und Sommer kann sie in der Abenddämmerung bei der Nahrungssuche beobachtet werden. Mittlerweile sind einige Arten vom Aussterben bedroht, weitere Arten gelten als stark gefährdet. Am Schutz der Tiere kann sich jedoch ganz leicht beteiligt werden: Das Anbringen künstlicher Quartiere und die Vermeidung von Pestiziden im Garten sind nur zwei der vielen Maßnahmen, die man ergreifen kann.<br _ngcontent-gqh-c102=\"\"><br _ngcontent-gqh-c102=\"\"> Man unterscheidet bei Fledermauskästen zwischen Flachkästen und sogenannten Raumkästen, die als Ersatz für natürliche Quartiere in Specht- und Asthöhlen dienen. Ein Fledermauskasten lässt sich recht einfach selbst bauen. Schritt-für-Schritt-Anleitungen findet man im Internet.<br _ngcontent-gqh-c102=\"\"><br _ngcontent-gqh-c102=\"\"> Verwendet unbehandeltes Holz mit rauer Oberfläche. Vor allem die Rückwand sollte stark aufgeraut sein, nur so finden die Fledermäuse daran Halt. Optimal ist ökozertifiziertes Holz. Das Einstiegloch sollte sich, anders als bei Vogelhäuschen, an der Unterseite befinden, denn die Fledermäuse klettern nach oben in den Kasten hinein. Der Fledermauskasten sollte in einer Höhe von mindestens 5 m angebracht werden, da sich Fledermäuse beim Losfliegen nach unten fallen lassen.<br _ngcontent-gqh-c102=\"\"><br _ngcontent-gqh-c102=\"\"> Hängt die Kästen wettergeschützt auf, idealerweise nach Süd-Osten ausgerichtet. Wenn ihr mehrere Kästen in verschiedene Himmelsrichtungen aufhängt, können die Fledermäuse je nach Temperatur zwischen den Kästen wechseln. Für den freien An- und Abflug sollten die Kästen nicht hinter Bäumen aufgehängt werden.<br _ngcontent-gqh-c102=\"\"><br _ngcontent-gqh-c102=\"\"> Wussten ihr, dass ihr euren Garten zusätzlich „fledermausfreundlich“ gestalten könnt? Fledermäuse ernähren sich von Insekten, allen voran von Nachtfaltern. Daher sind nachtblühende und nektarreiche Blütenpflanzen, zum Beispiel Seifenkraut, Leimkraut und Wegwarte optimal. Ihr intensiver Duft lockt Nachtfalter an, die ihrerseits die nachtaktiven Fledermäuse auf den Plan rufen. Auch ein Teich zieht viele Insekten an und bietet den Fledermäusen ein breites Nahrungsangebot. Ihr solltet auch auf Insektizide verzichten, sonst „klaut“ ihr den Fledermäusen ihre Nahrung. </p> <div _ngcontent-gqh-c102=\"\" class=\"row\"> <div _ngcontent-gqh-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-gqh-c102=\"\" src=\"https://gardify.de//assets/images/Fledermauskasten.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
            case "Vogelfutterstation":
                html_string = "<html><body><div _ngcontent-poj-c102=\"\" class=\"col-12 col-md-12\"> <h2 _ngcontent-poj-c102=\"\" class=\"t-13\" style=\"text-decoration: underline;\">Vogelfutterstation</h2> <p _ngcontent-poj-c102=\"\"> Unsere Gartenvögel haben inzwischen ganzjährig mit Futtermangel zu kämpfen. Naturschutzverbände empfehlen die Ganzjahresfütterung. Der wichtigste Naturschutz für unsere Vögel ist ein naturnaher Garten mit heimischen Pflanzen, doch gerade in der Brutzeit ist das Nahrungsangebot an Insekten knapp. Um einen sicheren Futterplatz zu gewährleisten, solltet ihr darauf achten, dass er trocken und sauber bleibt und vor Angreifern geschützt ist. Wer die folgenden Regeln beachtet, wird sich bald verschiedener Vogelarten erfreuen und das bunte Treiben von Blaumeise, Rotkehlchen, Grünfink und Co. ganz nah verfolgen können.<br _ngcontent-poj-c102=\"\"><br _ngcontent-poj-c102=\"\"> Die Futterstation sollte sich auf einer freien Fläche befinden, damit Katzen sich nicht so leicht anpirschen können, gleichzeitig sollten in einem angemessen Abstand Bäume oder Büsche Deckung vor Sperbern bieten. Achtet darauf, dass Glasscheiben in der Nähe für die Vögel nicht zur tödlichen Falle werden.<br _ngcontent-poj-c102=\"\"><br _ngcontent-poj-c102=\"\"> Futtersilos und -säulen sind optimal: Das gelagerte Futter wird nicht nass und die Vögel können, da sie nicht im Futter herumlaufen, ihre Futterstelle nicht verunreinigen. Andernfalls muss sie täglich gereinigt werden, um die Ausbreitung von Krankheiten und das Risiko einer Salmonellenübertragung zu verringern. Benutzt bei der Reinigung keine chemischen Mittel, sondern bürstet die Futterstelle zunächst aus und reinigt sie anschließend mit warmem Wasser. Tragt aus hygienischen Gründen bei der Reinigung der Futterstationen Gummihandschuhe.<br _ngcontent-poj-c102=\"\"><br _ngcontent-poj-c102=\"\"> Natürlich können die Futterstationen fertig gekauft werden. Anleitungen für die eigene DIY- Vogelfutterstation finden ihr auch im Internet, darunter auch solche, die man mit Kindern gemeinsam bauen kann. </p> <p _ngcontent-poj-c102=\"\"> Welcher Vogel mag welches Futter? </p> <div _ngcontent-poj-c102=\"\" class=\"table-responsive\"> <table _ngcontent-poj-c102=\"\" class=\"table table-bordered\"> <tbody _ngcontent-poj-c102=\"\"> <tr _ngcontent-poj-c102=\"\"> <td _ngcontent-poj-c102=\"\">Weichfutterfresser:&nbsp;<span _ngcontent-poj-c102=\"\" class=\"t-bold\">Amsel, Drossel, Rotkehlchen und Heckenbraunelle</span></td> <td _ngcontent-poj-c102=\"\">Äpfel, Rosinen, Getreideflocken und Mehlwürmer</td> </tr> <tr _ngcontent-poj-c102=\"\"> <td _ngcontent-poj-c102=\"\">Körnerfresser: <span _ngcontent-poj-c102=\"\" class=\"t-bold\">Buchfink, Bergfink, Erlenzeisig, Gimpel oder Kernbeißer</span></td> <td _ngcontent-poj-c102=\"\">Körnergemische, Erdnussbruch, Sonnenblumenkerne und energiereiche, ölhaltige Sämereien wie Nijer, Hanf oder Mohn</td> </tr> <tr _ngcontent-poj-c102=\"\"> <td _ngcontent-poj-c102=\"\"><span _ngcontent-poj-c102=\"\" class=\"t-bold\">Specht und Kleiber</span> </td> <td _ngcontent-poj-c102=\"\">Siehe Körnerfresser, sowie Fettfutter und Mehlwürmer</td> </tr> <tr _ngcontent-poj-c102=\"\"> <td _ngcontent-poj-c102=\"\"><span _ngcontent-poj-c102=\"\" class=\"t-bold\">Kohlmeise, Schwanzmeise, Sumpfmeise und Tannenmeise</span>&nbsp;fressen besonders gerne</td> <td _ngcontent-poj-c102=\"\">Erdnussbruch, Sonnenblumenkerne und Fettfutter, auch Mehlwürmer</td> </tr> <tr _ngcontent-poj-c102=\"\"> <td _ngcontent-poj-c102=\"\"><span _ngcontent-poj-c102=\"\" class=\"t-bold\">Hausperling und Feldsperling</span></td> <td _ngcontent-poj-c102=\"\">Körnergemischen, Erdnüssen und Sonnenblumenkernen</td> </tr> </tbody> </table> </div> <p _ngcontent-poj-c102=\"\"> Was gibt’s bei der Vogelfutterwahl noch zu beachten: Vogelfutter darf kein Salz enthalten, übriggebliebene Lebensmittel wie Käse, Schinken oder Wurst sind tabu. Brot eignet sich auch nicht, da es im Magen der Vögel aufquellen kann. Reines Fett, wie bspw. Butter oder Margarine, bekommt Vögeln ebenfalls nicht. Das Vogelfutter sollte möglichst frei von Ambrosia-Samen sein. Diese Beifuß-Art ist neu in Deutschland und kann bei Allergikern schwere Reaktionen herbeiführen. Billiges Vogelfutter wird oft mit Weizenkörnern gestreckt. Die Körner lassen die Vögel sogar im Winter liegen. Meisenknödel sollten nicht in Plastiknetzen eingewickelt sein, die Vögel können sich daran verletzen. Vergesst nicht, dass Vogelfutter irgendwo angebaut werden muss, meist in konventioneller Landwirtschaft, die kaum mehr Lebensraum für gefährdete Agrarvogelarten bietet. Es ist sinnvoll, Vogelfutter aus biologischem Anbau zu kaufen. Es ist qualitativ nicht besser als anderes, jedoch ökologisch vertretbar.<br _ngcontent-poj-c102=\"\"><br _ngcontent-poj-c102=\"\"> Wer sich die Mühe macht, den heimischen Vögeln mithilfe von zusätzlichem Nahrungsangebot zu helfen, sollte auch die eigene Gartengestaltung überdenken: Büsche, die Beeren tragen, sowie Obstbäume und darunter liegen gebliebenes Fallobst bieten Vögeln eine natürliche Futterquelle. Hecken und Gehölze beherbergen viele Insekten, die wiederum seltenere Vogelarten anlocken wie das Wintergoldhähnchen oder die Schwanzmeise. In Totholzhaufen befinden sich Kleinstlebewesen, an denen sich hungrige Vögel erfreuen. Es gibt zahlreiche Möglichkeiten, den eigenen Garten für die singenden Gäste attraktiv zu gestalten. </p> <div _ngcontent-poj-c102=\"\" style=\"border-color: #7a9d34; border-style: solid;\"> <p _ngcontent-poj-c102=\"\" style=\"padding: 10px;\"><strong _ngcontent-poj-c102=\"\">Achtung</strong>: Vor allem in den warmen Monaten kann es zu Infektionen der Tiere kommen. Solltet ihr einen kranken oder toten Vogel in der Nähe der Futterstation finden, können eine Salmonellose oder Trichomonaden der Grund dafür sein. Diese Krankheiten übertragen sich besonders schnell an den klassischen Futterhäuschen! Stellt die Fütterung sofort ein, säubert und desinfiziert mit starkverdünnter Essigessenz das Futterhaus und entfernt alle Futterreste am Boden. Tote Vögel sollten nur mit Gummihandschuhen angefasst werden. Mit Trichomonaden infizieren sich besonders häufig Grünfinken, deshalb sollte ein solcher Vorfall immer gemeldet werden. Krankheiten breiten sich schneller an großen Futterplätzen aus, daher ist es ratsam, mehrere kleine Futterstellen anzubieten. Es empfiehlt sich außerdem dann auf ein Silosystem umzustellen. </p> </div> <div _ngcontent-poj-c102=\"\" class=\"row\"> <div _ngcontent-poj-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-poj-c102=\"\" src=\"https://gardify.de/assets/images/vogelfutterstation1.jpg\" style=\"width: 100%;\"></div> <br> <div _ngcontent-poj-c102=\"\" class=\"col-md-12 mt-4 text-center\"><img _ngcontent-poj-c102=\"\" src=\"https://gardify.de/assets/images/vogelfutterstation2.jpg\" style=\"width: 100%;\"></div> </div> </div></body></html>";
                break;
        }
        return html_string;
    }

    MyGardenPersistDataViewModel persistDataViewModel;
    @Override
    public void onActivityCreated(@Nullable Bundle savedInstanceState) {
        super.onActivityCreated(savedInstanceState);
        persistDataViewModel = ViewModelProviders.of(getActivity()).get(MyGardenPersistDataViewModel.class);
        persistDataViewModel.setMyGardenState(R.string.all_ecoElements);

    }

}