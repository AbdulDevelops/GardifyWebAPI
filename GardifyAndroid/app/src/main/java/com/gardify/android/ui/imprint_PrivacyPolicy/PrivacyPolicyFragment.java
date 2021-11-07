package com.gardify.android.ui.imprint_PrivacyPolicy;

import android.os.Bundle;
import android.text.Html;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;

import com.gardify.android.R;

import static com.gardify.android.utils.UiUtils.setupToolbar;

public class PrivacyPolicyFragment extends Fragment {


    public View onCreateView(@NonNull LayoutInflater inflater,
                             ViewGroup container, Bundle savedInstanceState) {

        View root = inflater.inflate(R.layout.fragment_privacy_policy, container, false);

        setupToolbar(getActivity(), "Datenschutz", R.drawable.gardify_icon, R.color.colorPrimary,true);
        init(root);

        return root;
    }

    public void init(View root) {
        TextView privacyPolicyTextView = root.findViewById(R.id.text_view_privacy_policy);

        String htmlString = "Der Schutz Ihrer personenbezogenen Daten ist uns wichtig. Durch Ihre Verwendung dieser Website stimmen Sie der Erfassung, Nutzung und Übertragung Ihrer Daten gemäß dieser Datenschutzerklärung zu. Personenbezogene Daten sind alle Informationen zu einer namentlich genannten oder identifizierbaren Person. Looking for the english version? Click here" +
                "<br><br>" +
                "<h4> 1 Verantwortliche Stelle</h4>" +
                "BECKER JOEST VOLK VERLAG GmbH & Co. KG" +
                "<br>" +
                "Bahnhofsallee 5" +
                "<br>" +
                "40721 Hilden" +
                "<br>" +
                "Geschäftsführer: Ralf Joest, Andreas Grewe, Johanna Hänichen" +
                "<br><br>" +
                "<h4> 2 Allgemeine Nutzung der Webseite</h4>" +
                "<b> 2.1 Zugriffsdaten</b>" +
                "<br>" +
                "Wir sammeln Informationen über Sie, wenn Sie diese Webseite nutzen. Wir erfassen automatisch Informationen über Ihr Nutzungsverhalten und Ihre Interaktion mit uns und registrieren Daten zu Ihrem Computer oder Mobilgerät. Wir erheben, speichern und nutzen Daten über jeden Zugriff auf unser Onlineangebot (sogenannte Serverlogfiles). Zu den Zugriffsdaten gehören Name und URL der abgerufenen Datei, Datum und Uhrzeit des Abrufs, übertragene Datenmenge, Meldung über erfolgreichen Abruf (HTTP response code), Browsertyp und Browserversion, Betriebssystem, Referrer URL (d. h. die zuvor besuchte Seite), IP-Adresse und der anfragende Provider. Wir nutzen diese Protokolldaten ohne Zuordnung zu Ihrer Person oder sonstiger Profilerstellung für statistische Auswertungen zum Zweck des Betriebs, der Sicherheit und der Optimierung unseres Onlineangebotes, aber auch zur anonymen Erfassung der Anzahl der Besucher auf unserer Webseite (traffic) sowie zu Umfang und zur Art der Nutzung unserer Webseite und Dienste, ebenso zu Abrechnungszwecken, um die Anzahl der von Kooperationspartnern erhaltenen Clicks zu messen. Aufgrund dieser Informationen können wir personalisierte und standortbezogene Inhalte zur Verfügung stellen und den Datenverkehr analysieren, Fehler suchen und beheben und unsere Dienste verbessern. Wir behalten uns vor, die Protokolldaten nachträglich zu überprüfen, wenn aufgrund konkreter Anhaltspunkte der berechtigte Verdacht einer rechtswidrigen Nutzung besteht. IP-Adressen speichern wir für einen begrenzten Zeitraum in den Logfiles, wenn dies für Sicherheitszwecke erforderlich oder für die Leistungserbringung oder die Abrechnung einer Leistung nötig ist, z. B. wenn Sie eines unserer Angebote nutzen. Nach Abbruch des Vorgangs der Bestellung oder nach Zahlungseingang löschen wir die IP-Adresse, wenn diese für Sicherheitszwecke nicht mehr erforderlich sind. IP-Adressen speichern wir auch dann, wenn wir den konkreten Verdacht einer Straftat im Zusammenhang mit der Nutzung unserer Website haben. Außerdem speichern wir als Teil Ihres Accounts das Datum Ihres letzten Besuchs (z. B. bei Registrierung, Login, Klicken von Links etc.).\n" +
                "Grundsätzlich ist die Bereitstellung solcher technischen Daten weder gesetzlich noch vertraglich vorgeschrieben oder für einen Vertragsabschluss erforderlich. Sie sind nicht verpflichtet, die personenbezogenen Daten bereitzustellen. Wenn Ihr System die erforderlichen Informationen nicht oder nicht vollständig zur Verfügung stellt, kann das dazu führen, dass unsere Website nicht oder nicht vollständig aufgerufen werden kann.\n" +
                "<br>" +
                "<br>" +
                "<b> 2.2 E-Mail / Kontakt</b>" +
                "<br>" +
                "Wenn Sie mit uns in Kontakt treten (z. B. per Kontaktformular oder E-Mail), speichern wir Ihre Angaben zur Bearbeitung der Anfrage sowie für den Fall, dass Anschlussfragen entstehen. Weitere personenbezogene Daten speichern und nutzen wir nur, wenn Sie dazu einwilligen oder dies ohne besondere Einwilligung gesetzlich zulässig ist.\n" +
                "<br>" +
                "<br>" +
                "<b> 2.3 Newsletter-Versand</b>" +
                "<br>" +
                "Falls Sie unseren Newsletter abonniert haben, verwenden wir mit Ihrer Einwilligung Ihre personenbezogenen Daten, insbesondere Ihre E-Mail-Adresse und Ihren Namen, um den Newsletter übermitteln zu können. Sie können die Einwilligung jederzeit ohne Angabe von Gründen mit Wirkung für die Zukunft widerrufen. Die Datenverarbeitung erfolgt auf der Grundlage von Art. 6. Abs. 1 lit. a DSGVO. Ihre Daten speichern wir so lange, bis Sie den Newsletter wieder abbestellen bzw. die Einwilligung widerrufen.\n" +
                "<br>" +
                "<br>" +
                "<b> 2.4 Cookies</b>" +
                "<br>" +
                "Für unseren Internetauftritt nutzen wir Session Cookies. Cookies sind Textdateien, die im Rahmen Ihres Besuchs unserer Internetseiten von unserem Webserver an Ihren Browser gesandt und von diesem auf Ihrem Rechner für einen späteren Abruf vorgehalten werden. Ihr Name wird dabei nicht übermittelt. Ob wir Cookies einsetzen können, können Sie durch die Einstellungen in Ihrem Browser selbst bestimmen. Sie können in Ihrem Browser das Speichern von Cookies vollständig deaktivieren, es auf bestimmte Websites beschränken oder Ihren Browser so konfigurieren, dass er Sie automatisch benachrichtigt, sobald ein Cookie gesetzt werden soll. Eine Erhebung oder Speicherung personenbezogener Daten in Cookies findet in diesem Zusammenhang durch uns nicht statt. Bei den von uns eingesetzten Cookies handelt es sich ausschließlich um sogenannte Session-Cookies, die ein Tracking des Nutzungsverhaltens nicht ermöglichen. Wir setzen auch keine Techniken ein, die durch Cookies anfallende Informationen – etwa die IP-Adresse – mit Nutzerdaten verbinden. Die Verarbeitung erfolgt auf der Grundlage von Art. 6 Abs. 1 b DSGVO. Soweit Cookies von uns eingesetzt werden, werden sie beim Beenden der Browsersitzung automatisch gelöscht. Detaillierte Informationen zu den von uns eingesetzten Cookies können Sie Ihrem Browser entnehmen. Cookies lassen sich zudem über eine von Ihrem Browser bereitgestellte Löschfunktion von Ihrem System manuell entfernen. Der Einsatz von Cookies ist weder gesetzlich noch vertraglich vorgeschrieben und für einen Vertragsabschluss auch nicht erforderlich. Sie sind nicht verpflichtet ist, die personenbezogenen Daten bereitzustellen. Wenn Sie allerdings den Einsatz von Cookies nicht zulassen, kann das dazu führen, dass einige Funktionen der Website nicht oder nicht richtig funktionieren und/oder bestimmte Inhalte nicht oder nicht richtig angezeigt werden.\n" +
                "<br>" +
                "<br>" +
                "<b> 2.5 Google Analytics </b>" +
                "<br>" +
                "Wir nutzen den Dienst »Google Analytics«, der von der Google Inc. (1600 Amphitheatre Parkway Mountain View, CA 94043, USA) angeboten wird, zur Analyse der Websitebenutzung durch Nutzer. Google Analytics verwendet sog. „Cookies“, Textdateien, die auf Ihrem Computer gespeichert werden und die eine Analyse der Benutzung der Website durch Sie ermöglichen. Die durch den Cookie erzeugten Informationen über Benutzung dieser Website durch die Seitenbesucher werden in der Regel an einen Server von Google in den USA übertragen und dort gespeichert. Im Falle der Aktivierung der IP-Anonymisierung auf dieser Webseite, wird Ihre IP-Adresse von Google jedoch innerhalb von Mitgliedstaaten der Europäischen Union oder in anderen Vertragsstaaten des Abkommens über den Europäischen Wirtschaftsraum zuvor gekürzt. Nur in Ausnahmefällen wird die volle IP-Adresse an einen Server von Google in den USA übertragen und dort gekürzt. Die IP-Anonymisierung ist auf dieser Website aktiv. In unserem Auftrag wird Google diese Informationen benutzen, um die Nutzung der Website durch Sie auszuwerten, um Reports über die Websiteaktivitäten zusammenzustellen und um weitere mit der Websitenutzung und der Internetnutzung verbundene Dienstleistungen uns gegenüber zu erbringen. Die im Rahmen von Google Analytics von Ihrem Browser übermittelte IP-Adresse wird nicht mit anderen Daten von Google zusammengeführt. Sie können die Speicherung der Cookies durch eine entsprechende Einstellung Ihrer Browser-Software verhindern; wir weisen Sie jedoch darauf hin, dass Sie in diesem Fall gegebenenfalls nicht sämtliche Funktionen dieser Website vollumfänglich werden nutzen können. Sie können darüber hinaus die Erfassung der durch das Cookie erzeugten und auf ihre Nutzung der Website bezogenen Daten (inkl. Ihrer IP-Adresse) an Google sowie die Verarbeitung dieser Daten durch Google verhindern, indem Sie das unter dem folgenden Link verfügbare Browser-Plugin herunterladen und installieren: http://tools.google.com/dlpage/gaoptout?hl=de. Alternativ zum Browser-Plugin oder innerhalb von Browsern auf mobilen Geräten können Sie auf den folgenden Link klicken, um ein Opt-Out-Cookie zu setzen, der die Erfassung durch Google Analytics innerhalb dieser Website zukünftig verhindert (dieses Opt-Out-Cookie funktioniert nur in diesem Browser und nur für diese Domain. Löschen Sie die Cookies in Ihrem Browser, müssen Sie diesen Link erneut klicken): Google Analytics deaktivieren\n" +
                "<br>" +
                "<br>" +
                "<b> 2.6 Google-Tag-Manager </b>" +
                "<br>" +
                "Diese Website benutzt den Google Tag Manager. Google Tag Manager ist eine Lösung, mit der Vermarkter Website-Tags über eine Oberfläche verwalten können. Das Tool Tag Manager selbst (das die Tags implementiert) ist eine cookielose Domain und erfasst keine personenbezogenen Daten. Das Tool sorgt für die Auslösung anderer Tags, die ihrerseits unter Umständen Daten erfassen. Google Tag Manager greift nicht auf diese Daten zu. Wenn auf Domain- oder Cookie-Ebene eine Deaktivierung vorgenommen wurde, bleibt diese für alle Tracking-Tags bestehen, die mit Google Tag Manager implementiert werden.\n" +
                "<br>" +
                "<br>" +
                "<b> 2.7 Rechtsgrundlagen und Speicherdauer</b>" +
                "<br>" +
                "Rechtsgrundlage der Datenverarbeitung nach den vorstehenden Ziffern ist Art 6 Abs. 1 Buchstabe f) DSGVO. Unsere Interessen an der Datenverarbeitung sind insbesondere die Sicherstellung des Betriebs und der Sicherheit der Webseite, die Untersuchung der Art und Weise der Nutzung der Webseite durch Besucher und die Vereinfachung der Nutzung der Webseite. Sofern nicht spezifisch angegeben speichern wir personenbezogene Daten nur so lange, wie dies zur Erfüllung der verfolgten Zwecke notwendig ist.\n" +
                "<br>" +
                "<h4> 3 Ihre Rechte als von der Datenverarbeitung Betroffener </h4>" +
                "<b> 3.1 Recht auf Bestätigung und Auskunft </b>" +
                "<br>" +
                "Sie haben jederzeit das Recht, von uns eine Bestätigung darüber zu erhalten, ob Sie betreffende, personenbezogene Daten verarbeitet werden. Ist dies der Fall, so haben Sie das Recht, von uns eine unentgeltliche Auskunft über die zu Ihnen gespeicherten personenbezogenen Daten nebst einer Kopie dieser Daten zu erlangen. Des Weiteren besteht ein Recht auf folgende Informationen:\n" +
                "<br>" +
                "<ul>" +
                "<il> 1. zu den Verarbeitungszwecken; </il> " +
                "<il> 2. zu den Kategorien personenbezogener Daten, die verarbeitet werden;</il>" +
                "<br>" +
                "<il> 3. zu Empfängern oder Kategorien von Empfängern, gegenüber denen die personenbezogenen Daten offengelegt worden sind oder noch offengelegt werden, insbesondere bei Empfängern in Drittländern oder bei internationalen Organisationen; </il>" +
                "<br>" +
                "<il> 4. zur geplanten Dauer wenn möglich, für die die personenbezogenen Daten gespeichert werden, oder, falls dies nicht möglich ist, die Kriterien für die Festlegung dieser Dauer; </il> " +
                "<br>" +
                "<il> 5. zum Bestehen eines Rechts auf Berichtigung oder Löschung der Sie betreffenden personenbezogenen Daten oder auf Einschränkung der Verarbeitung durch den Verantwortlichen oder eines Widerspruchsrechts gegen diese Verarbeitung; </il>" +
                "<br>" +
                "<il> 6. zum Bestehen eines Beschwerderechts bei einer Aufsichtsbehörde;</il>" +
                "<br>" +
                "<il> 7. zu personenbezogenen Daten, die nicht bei Ihnen erhoben werden, alle verfügbaren Informationen über die Herkunft der Daten. </il>" +
                "</ul>" +
                "<br>" +
                "<br>" +
                "<b> 3.2 Recht auf Berichtigung </b>" +
                "<br>" +
                "Sie haben das Recht, von uns unverzüglich die Berichtigung Sie betreffender unrichtiger personenbezogener Daten zu verlangen. Unter Berücksichtigung der Zwecke haben Sie das Recht, die Vervollständigung unvollständiger personenbezogener Daten – auch mittels einer ergänzenden Erklärung – zu verlangen.\n" +
                "<br>" +
                "<br>" +
                "<b> 3.3 Recht auf Löschung </b>" +
                "<br>" +
                "Sie haben das Recht, von uns zu verlangen, dass Sie betreffende personenbezogene Daten unverzüglich gelöscht werden, und wir sind verpflichtet, personenbezogene Daten unverzüglich zu löschen, sofern einer der folgenden Gründe zutrifft:\n" +
                "<br>" +
                "<ul>" +
                "<il> 1. Die personenbezogenen Daten sind für die Zwecke, für die sie erhoben oder auf sonstige Weise verarbeitet wurden, nicht mehr notwendig.\n" +
                "<br>" +
                "<il>  2. Sie widerrufen Ihre Einwilligung, auf die sich die Verarbeitung gemäß Artikel 6 Absatz 1 DSGVO Buchstabe a oder Artikel 9 Absatz 2 Buchstabe a DSGVO stützte, und es fehlt an einer anderweitigen Rechtsgrundlage für die Verarbeitung.\n" +
                "<br>" +
                "<il>  3. Sie legen gemäß Artikel 21 Absatz 1 DSGVO Widerspruch gegen die Verarbeitung ein und es liegen keine vorrangigen berechtigten Gründe für die Verarbeitung vor, oder Sie legen gemäß Artikel 21 Absatz 2 DSGVO Widerspruch gegen die Verarbeitung ein.\n" +
                "<br>" +
                "<il>  4. Die personenbezogenen Daten wurden unrechtmäßig verarbeitet.\n" +
                "<br>" +
                "<il> 5. Die Löschung der personenbezogenen Daten ist zur Erfüllung einer rechtlichen Verpflichtung nach dem Unionsrecht oder dem Recht der Mitgliedstaaten erforderlich, dem wir unterliegen.\n" +
                "<br>" +
                "<il> 6. Die personenbezogenen Daten wurden in Bezug auf angebotene Dienste der Informationsgesellschaft gemäß Artikel 8 Absatz 1 DSGVO erhoben. Haben wir die personenbezogenen Daten öffentlich gemacht und sind wir zu deren Löschung verpflichtet, so treffen wir unter Berücksichtigung der verfügbaren Technologie und der Implementierungskosten angemessene Maßnahmen, auch technischer Art, um für die Datenverarbeitung Verantwortliche, die die personenbezogenen Daten verarbeiten, darüber zu informieren, dass Sie von ihnen die Löschung aller Links zu diesen personenbezogenen Daten oder von Kopien oder Replikationen dieser personenbezogenen Daten verlangt hat.\n" +
                "<br>" +
                "<il> 7. zu personenbezogenen Daten, die nicht bei Ihnen erhoben werden, alle verfügbaren Informationen über die Herkunft der Daten;\n" +
                "</ul>" +
                "<b> 3.4 Recht auf Einschränkung der Verarbeitung</b> " +
                "<br>" +
                "Sie haben das Recht, von uns die Einschränkung der Verarbeitung zu verlangen, wenn eine der folgenden Voraussetzungen gegeben ist:\n" +
                "<ul>" +
                "<il> 1. die Richtigkeit der personenbezogenen Daten wird von Ihnen bestritten, und zwar für eine Dauer, die es uns ermöglicht, die Richtigkeit der personenbezogenen Daten zu überprüfen,\n" +
                "<br>" +
                "<il> 2. die Verarbeitung unrechtmäßig ist und Sie die Löschung der personenbezogenen Daten ablehnten und stattdessen die Einschränkung der Nutzung der personenbezogenen Daten verlangt;\n" +
                "<br>" +
                "<il> 3. wir die personenbezogenen Daten für die Zwecke der Verarbeitung nicht länger benötigen, Sie die Daten jedoch zur Geltendmachung, Ausübung oder Verteidigung von Rechtsansprüchen benötigten, oder\n" +
                "<br>" +
                "<il> 4. Sie haben Widerspruch gegen die Verarbeitung gemäß Artikel 21 Absatz 1 DSGVO eingelegt, solange noch nicht feststeht, ob die berechtigten Gründe unseres Unternehmens gegenüber den Ihren überwiegen.\n" +
                "</ul>" +
                "<b> 3.5 Recht auf Datenübertragbarkeit </b>" +
                "<br>" +
                "Sie haben das Recht, die Sie betreffenden personenbezogenen Daten, die Sie uns bereitgestellt haben, in einem strukturierten, gängigen und maschinenlesbaren Format zu erhalten, und Sie haben das Recht, diese Daten einem anderen Verantwortlichen ohne Behinderung durch uns zu übermitteln, sofern\n" +
                "<ul>" +
                "<il> 1. die Verarbeitung auf einer Einwilligung gemäß Artikel 6 Absatz 1 Buchstabe a DSGVO oder Artikel 9 Absatz 2 Buchstabe a DSGVO oder auf einem Vertrag gemäß Artikel 6 Absatz 1 Buchstabe b DSGVO beruht und</il>" +
                "<il> 2. die Verarbeitung mithilfe automatisierter Verfahren erfolgt. Bei der Ausübung ihres Rechts auf Datenübertragbarkeit gemäß Absatz 1 haben Sie das Recht, zu erwirken, dass die personenbezogenen Daten direkt von uns einem anderen Verantwortlichen übermittelt werden, soweit dies technisch machbar ist.</il>" +
                "</ul>" +
                "<b> 3.6 Widerspruchsrecht </b>" +
                "<br>" +
                "Sie haben das Recht, aus Gründen, die sich aus Ihrer besonderen Situation ergeben, jederzeit gegen die Verarbeitung sie betreffender personenbezogener Daten, die aufgrund von Artikel 6 Absatz 1 Buchstaben e oder f DSGVO erfolgt, Widerspruch einzulegen; dies gilt auch für ein auf diese Bestimmungen gestütztes Profiling. Wir verarbeiten die personenbezogenen Daten nicht mehr, es sei denn, wir können zwingende schutzwürdige Gründe für die Verarbeitung nachweisen, die Ihre Interessen, Rechte und Freiheiten überwiegen, oder die Verarbeitung dient der Geltendmachung, Ausübung oder Verteidigung von Rechtsansprüchen. Werden personenbezogene Daten von uns verarbeitet, um Direktwerbung zu betreiben, so haben Sie das Recht, jederzeit Widerspruch gegen die Verarbeitung Sie betreffender personenbezogener Daten zum Zwecke derartiger Werbung einzulegen; dies gilt auch für das Profiling, soweit es mit solcher Direktwerbung in Verbindung steht. Sie haben das Recht, aus Gründen, die sich aus Ihrer besonderen Situation ergeben, gegen die Sie betreffende Verarbeitung sie betreffender personenbezogener Daten, die zu wissenschaftlichen oder historischen Forschungszwecken oder zu statistischen Zwecken gemäß Artikel 89 Absatz 1 DSGVO erfolgt, Widerspruch einzulegen, es sei denn, die Verarbeitung ist zur Erfüllung einer im öffentlichen Interesse liegenden Aufgabe erforderlich.\n" +
                "<br>" +
                "<br>" +
                "<b>  3.7 Automatisierte Entscheidungen einschließlich Profiling </b>" +
                "Sie haben das Recht, nicht einer ausschließlich auf einer automatisierten Verarbeitung – einschließlich Profiling – beruhenden Entscheidung unterworfen zu werden, die Ihnen gegenüber rechtliche Wirkung entfaltet oder Sie in ähnlicher Weise erheblich beeinträchtigt.\n" +
                "<br>" +
                "<br>" +
                "<b> 3.8 Recht auf Widerruf einer datenschutzrechtlichen Einwilligung </b> " +
                "Sie haben das Recht, eine Einwilligung zur Verarbeitung personenbezogener Daten jederzeit zu widerrufen.\n" +
                "<br>" +
                "<br>" +
                "<b>  3.9 Recht auf Beschwerde bei einer Aufsichtsbehörde </b>" +
                "Sie haben das Recht auf Beschwerde bei einer Aufsichtsbehörde, insbesondere in dem Mitgliedstaat ihres Aufenthaltsorts, ihres Arbeitsplatzes oder des Orts des mutmaßlichen Verstoßes, Sie der Ansicht sind, dass die Verarbeitung der Sie betreffenden personenbezogenen Daten rechtswidrig ist.\n" +
                "<br>" +
                "<h4> 4 Datensicherheit </h4>" +
                "Wir sind um die Sicherheit Ihrer Daten im Rahmen der geltenden Datenschutzgesetze und technischen Möglichkeiten maximal bemüht. Ihre persönlichen Daten werden bei uns verschlüsselt übertragen. Dies gilt für Ihre Bestellungen und auch für das Kundenlogin. Wir nutzen das Codierungssystems SSL (Secure Socket Layer), weisen jedoch darauf hin, dass die Datenübertragung im Internet (z. B. bei der Kommunikation per E-Mail) Sicherheitslücken aufweisen kann. Ein lückenloser Schutz der Daten vor dem Zugriff durch Dritte ist nicht möglich. Zur Sicherung Ihrer Daten unterhalten wir technische- und organisatorische Sicherungsmaßnahmen, die wir immer wieder dem Stand der Technik anpassen. Wir gewährleisten außerdem nicht, dass unser Angebot zu bestimmten Zeiten zur Verfügung steht; Störungen, Unterbrechungen oder Ausfälle können nicht ausgeschlossen werden. Die von uns verwendeten Server werden regelmäßig sorgfältig gesichert.\n" +
                "<br>" +
                "<h4> 5 Weitergabe von Daten an Dritte, Keine Datenübertragung ins Nicht-EU-Ausland </h4>" +
                "Grundsätzlich verwenden wir Ihre personenbezogenen Daten nur innerhalb unseres Unternehmens. Wenn und soweit wir Dritte im Rahmen der Erfüllung von Verträgen einschalten (etwa Logistik-Dienstleister) erhalten diese personenbezogene aten nur in dem Umfang, in welchem die Übermittlung für die entsprechende Leistung erforderlich ist. Für den Fall, dass wir bestimmte Teile der Datenverarbeitung auslagern („Auftragsverarbeitung“), verpflichten wir Auftragsverarbeiter vertraglich dazu, personenbezogene Daten nur im Einklang mit den Anforderungen der Datenschutzgesetze zu verwenden und den Schutz der Rechte der betroffenen Person zu gewährleisten. Eine Datenübertragung an Stellen oder Personen außerhalb der EU außerhalb der in dieser Erklärung in Punkt 2.5 genannten Fällen findet nicht statt und ist nicht geplant.\n" +
                "<br>" +
                "<h4> 6 Links zu anderen Anbietern </h4>" +
                "Bitte beachten Sie, dass unsere Seiten Links zu Webseiten anderer Anbieter enthalten können, auf die sich diese Datenschutzerklärung nicht erstreckt.";

        privacyPolicyTextView.setText(Html.fromHtml(htmlString));
    }

}