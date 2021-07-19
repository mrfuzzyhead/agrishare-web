<%@ MasterType TypeName="Agrishare.Web.Pages.Default" %>
<%@ Page Title="Privacy Policy" Language="C#" MasterPageFile="~/Pages/Default.Master" AutoEventWireup="true" %>

<asp:Content ContentPlaceHolderID="Content" runat="server">
    <p ng-init="language = 'english'">
        <a ng-click="language = 'english'">English</a>&nbsp;/&nbsp;
        <a ng-click="language = 'shona'">Shona</a>&nbsp;/&nbsp;
        <a ng-click="language = 'ndebele'">Ndebele</a>
    </p>

    <div id="english" ng-show="language === 'english'">
        <h1>AgriShare Privacy Notice</h1>
    
        <p>We, Zimbabwe Welthungerhilfe, take the protection of your personal data seriously. We treat your personal data 
            confidentially and, naturally, in accordance with statutory data protection regulations and this Privacy Notice.</p>
    
        <h2>1. General Information</h2>
        <p>In this Privacy Notice, we explain the kind, scope, and purpose of the processing of personal data within the 
            Agrishare application, its related websites, functions, and content (hereinafter referred to as “AgriShare”). 
            This Privacy Notice applies regardless of the individual domains, systems, platforms, and devices (e.g., 
            desktop or mobile devices) on which our online services are accessed.</p>
    
        <p>This Privacy Notice governs data processing by:<br />
            Deutsche Welthungerhilfe e. V.<br />
            Friedrich-Ebert-Str. 1<br />
            53173 Bonn<br />
            Germany</p>
    
        <p>Terms such as &quot;personal data&quot; or the &quot;processing&quot; of the same are defined as per § 4 of
            the General Data Protection Regulation (GDPR). We process our users’ personal data only in accordance with 
            the relevant data protection regulations. This includes processing the users’ personal data on a legal basis,
            based on the users’ informed consent (§ 6.1(f) GDPR); based on our legitimate interests (§ 6.1(f) GDPR) if so 
            required i.e., interests of analysis, optimisation, and commercial operation and security of our online 
            services; or to fulfil our legal obligations (§ 6.1(c) GDPR).</p>
    
        <p>In this context, the term “user” comprises all categories of persons affected by the data processing; this 
            includes visitors of our website and user of the <i>AgriShare </i>application. The terms used, e.g., “user”, 
            are gender neutral.</p>
    
        <h2>2. What is your “personal data?”</h2>
    
        <p>We define “personal data” as any data or information that directly identifies you such as:</p>
    
        <ul>
            <li>your given name, date of birth, gender and your preferred language</li>
            <li>your financial information, this includes your bank account, your mobile money number</li>
            <li>your national identity number, or any identifying number assigned to you, including membership 
                numbers or promotional numbers</li>
            <li>your personal opinions on or views or preferences on how you use our products and services, 
                collected during marketing or opinion surveys</li>
            <li>your phone number used to register for our products and services, including any numbers listed 
                for sending or receipt of information, or transactions, message services, and voice calls.</li>
            <li>your email address used to register for our products and services, including any email address 
                listed on our mailing lists for promotional or marketing or other related services.</li>
            <li>your consent records as you may have given to allow us to collect the above information,</li>
        </ul>
    
        <p>We also treat other data that does not directly identify you but that can be reasonably used to identify 
            you, the same way we treat your personal data for example,</p>
    
        <ul>
            <li>your device’s IP address</li>
            <li>your device registration details, International Mobile Equipment Identity (IMEI)</li>
            <li>your subscriber identification module (SIM) card number</li>
            <li>your device location coordinates</li>
        </ul>
    
        <h2>3. How do we collect data from you?</h2>
    
        <p>We only collect necessary data that we need to ensure that <i>AgriShare </i>remains a great product. 
            These are the ways in which we collect personal data with your consent:</p>
    
        <ul>
            <li>when you create an account by providing your name, date of birth, and location, and any other 
                personal identifiable information including your national identity numbers, your phone number,
                or email addresses</li>
            <li>when you open the <i>AgriShare </i>mobile phone App, as it will immediately collect the 
                information about your location, whether you are transacting or not</li>
            <li>when the App requests access to your location data to ensure relevant listings to your 
                geographical location</li>
            <li>when you search listings offering equipment for hire, add listings for equipment for hire, 
                send and receive notifications</li>
            <li>when you make a booking for services and goods listed in the App</li>
            <li>when you add your mobile money details, your banking details or account information.</li>
            <li>when you transact on the <i>AgriShare </i>service, we will know your mobile money number 
                and the transactions you carry out</li>
            <li>when you interact with our platforms, website and applications even if you are not a 
                customer or have registered an account</li>
        </ul>
    
        <p>The data we have includes the following categories of information:</p>
        <ul>
            <li><b>Account Information. </b>Your <i>AgriShare </i>account details, including email address, 
                location, devices registered, and account status.</li>
            <li><b>Device Information.</b> Data about your device such as device serial number, 
                location and device model</li>
            <li><b>Transaction Information.</b> Data about transactions purchases you make on 
                the <i>AgriShare</i> and any other transactions facilitated on the platform.</li>
            <li><b>Usage Data. </b>Data about how you use the <i>AgriShare </i>App and from the Apps 
                information/ hosting website, including navigation history; search history; crash data, 
                performance and other diagnostic data; and other usage data.</li>
            <li><b>User experience:</b> opinion surveys, response to queries or any information you 
                volunteer in respect of the use of our platforms</li>
            <li><b>Online Marketing:</b> through our marketing campaigns, on online search 
                facilities, webpages, email. This information is collected by ads based on clicks, 
                including the device used</li>
            <li><b>Social media usage:</b> by looking through the list of social media users following our 
                pages, we will collect your social media contacts, to enhance our understanding of our 
                target audience whether you are a customer or not.</li>
        </ul>
        
        <h2>4. What do we do with your data once we have it?</h2>
    
        <p>We use your data to for the specific reasons that it was collected for, as this is consistent 
            with the provisions of the data protection laws to;</p>
    
        <ul>
            <li>avail services offered on our platforms, and applications</li>
            <li>help us provide and improve our software and services for you.</li>
            <li>helps us connect you to service providers, and third-party service providers</li>
            <li>help us disseminate our <i>AgriShare </i>performance reports, and for <i>AgriShare </i>
                promotional purposes without disclosing your personal data</li>
            <li>comply with all relevant local laws that require collection of some personal information, 
                such as the Banking Act, associated regulations, Know Your Client (KYC),</li>
            <li>implement security, and safety measures to prevent and detect fraud, other online crimes, 
                such as identity theft,</li>
            <li>improve our relationship management with you as a client, on services and products, 
                and for development and improvement of the products and services, to meet your 
                experiential needs,</li>
            <li>undertake internal reporting and implement business controls.</li>
        </ul>
        
        <h2>5. When do we share your data with others?</h2>
    
        <p>Data generated from the use of the App is also accessible to our development partner the 
            Community Technology Development Organisation (CTDO).</p>
    
        <p>We also employ any external service providers to provide our services, this is done in 
            accordance with the stipulations of § 28 GDPR. We have concluded a limited-processing 
            contract with each data processing company to ensure that your personal data are processed 
            according to their purpose and not shared. We also take appropriate technical and 
            organisational measures to protect your personal data.</p>
    
        <p>This applies to service providers including but not limited to the following:</p>
    
        <ul>
            <li>C2 Digital, Harare, Zimbabwe: Website maintenance and development</li>
            <li>Zimbabwe Farmers Union, 5 Van Praagh Avenue, Milton Park.Harare</li>
            <li>Custom in-house cookie management tool for Cookie Consent Management</li>
            <li>Google Inc., 1600 Amphitheatre Parkway, Mountain View, CA 94043, USA: Website 
                analysis and remarketing</li>
        </ul>
        
        <p>Insofar as contents, tools, or other materials are employed within the framework of this 
            Privacy Notice by other providers (hereinafter collectively referred to as “Third-Party 
            Providers”) registered in a third country, it must be assumed that data are being transferred
            to the Third-Party Provider&#39;s country of record. Third countries are countries in 
            which the GDPR is not directly-applicable law—categorically, this includes countries 
            outside of the European Union and the European Economic Area. Data may be transferred 
            into third countries in the presence of the appropriate data protection level, user 
            consent, or another form of statutory authorisation.</p>
    
        <p>Where the law requires it, for example during a criminal investigation we will be obliged 
            to turn over information that may include personal information to the investigating 
            entities once we are reasonably satisfied that they have the lawful permission to access 
            such information.</p>
    
        <h2>6. How do we store and protect your personal data?</h2>
    
        <p>We are committed to protecting your personal data once we have it. We implement physical, 
            business, and technical security measures. Despite our efforts, if we learn of a security
            breach, we will notify you so that you can take appropriate protective steps.</p>
    
        <p>We also do not want your personal data for any longer than we need it, so we only keep it 
            long enough to do what we collected it for. Once we do not need it, we take steps to 
            destroy it unless we are required by law to keep it longer. Personal data from deactivated 
            accounts is archived for six years in terms of the Postal and Telecommunications Act of 
            Zimbabwe; it is permanently erased from our servers at the lapse of that six-year period.</p>
    
        <p>The data that we store are deleted as soon as they are no longer required for their intended
            purpose and erasure does not violate any statutory retention periods. Insofar as user data
            are not deleted because they are required for other legal purposes, the processing of the 
            same is restricted. This means that the data are segregated and not processed for other 
            purposes. This applies, for example, to user data that need to be stored for commercial or 
            tax purposes.</p>
    
        <p>In accordance with statutory regulations, data pursuant to § 257.1 HGB (German Commercial 
            Code) must be retained for six years (annual financial statements, vouchers etc.), and 
            data pursuant to § 147.1 AO (German Revenue Code) must be retained for ten years (books, 
            records, management reports, vouchers, documentation relevant to taxation etc.).</p>
    
        <p><i>AgriShare</i> may keep non-personal and anonymised data for longer periods for reporting 
            purposes and in situations where laws make it necessary to keep such data.</p>
    
        <p>We store your data on servers located in Zimbabwe, additionally this data is backed up to 
            servers in Frankfurt, Germany. By giving us information, you consent to this kind of 
            transfer of your data. We comply with applicable law and will also abide by the 
            commitments we make in this privacy notice.</p>
    
        <h2>7. What are your rights?</h2>
    
        <p>Upon request and authentication, we are happy to inform you in writing and in accordance 
            with the applicable law if personal data concerning yourself have been processed. If this
            is the case, you are entitled to the disclosure of these personal data and of the 
            information listed in detail in § 15 GDPR.</p>
    
        <p>Should the personal data concerning yourself be incorrect, you have the right to immediately 
            demand the rectification of inaccurate personal data or the completion of incomplete 
            personal data (§ 16 GDPR).</p>
    
        <p>You have the right to demand that personal data concerning yourself be immediately deleted 
            insofar as one of the particular reasons listed in § 17 GDPR applies, for example if the 
            data are no longer required for their intended purpose (right to erasure).</p>
    
        <p>You also have the right to demand the restriction of processing if one of the prerequisites 
            listed in § 18 GDPR applies; for example, if you have objected to processing, this will
            apply for the duration of examination by the controller.</p>
    
        <p>You have the right to object at any time to the processing of personal data concerning 
            yourself for reasons arising from your particular situation. Our company will then cease 
            to process said personal data unless we can prove compelling grounds for the processing or 
            the processing serves to establish, exercise, or defend legal claims (§ 21 GDPR).</p>
    
        <p>Without prejudice to any other administrative or judicial remedy, you have the right to
            file a complaint with the supervisory authority if you believe that the processing of 
            personal data concerning yourself infringes the GDPR (§ 77 GDPR). You may exercise this 
            right through a supervisory authority in the member state of your place of residence, of 
            your place of employment, or of the place in which the alleged infraction occurred. In 
            North Rhine– Westphalia, the following person has jurisdiction:</p>
    
        <p>The data protection officer for North Rhine–Westphalia (LDI)<br />
            Kavalleriestrasse 2 - 4<br />
            40213 Düsseldorf Germany</p>
            
        <h2 id="cookies">8. Cookies and Reach Measurement</h2>
    
        <p>Cookies are data which are transmitted from our web server or from third-party web servers 
            to the user&#39;s web browser and stored there for future retrieval. Cookies may take the
            form of small files or other forms of information storage. In accordance with this Privacy
            Notice, you will be informed of any cookies employed for pseudonymous reach measurement.</p>
    
        <p>If you do not want any cookies to be stored on your computer, you are requested to 
            deactivate the corresponding option in your browser’s system settings. Stored cookies can 
            be cleared in the browser’s system settings. Deactivating cookies may limit the Website’s 
            functionality.</p>
    
        <p><a href="https://support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies" target="_blank">Internet Explorer:
            support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies</a></p>
    
        <p><a href="https://support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences" target="_blank">Firefox:
            support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences</a></p>
    
        <p><a href="https://support.google.com/chrome/answer/95647?hl=en-GB" target="_blank">Google Chrome:
            support.google.com/chrome/answer/95647?hl=en-GB</a></p>
    
        <p><a href="https://support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac" target="_blank">Safari:
            support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac</a></p>
    
        <p><a href="http://help.opera.com/Linux/12.10/en/cookies.html" target="_blank">Opera:
            help.opera.com/Linux/12.10/en/cookies.html</a></p>
    
        <p>You can also opt out of the use of cookies for reach measurement and marketing purposes via the Network Advertising 
            Initiative’s deactivation page (<a href="http://optout.networkadvertising.org/" target="_blank">http://optout.networkadvertising.org/</a>)
            and via the US website (<a href="http://www.aboutads.info/choices" target="_blank">http://www.aboutads.info/choices</a>)
            or the European website (<a href="http://www.youronlinechoices.com/uk/your-ad-choices/" target="_blank">http://www.youronlinechoices.com/uk/your-ad-choices/</a>).</p>
    
        <p>We also use the Usercentrics Consent Management Platform service from Usercentrics GmbH. This is a consent management 
            service that lists the purposes of data collection and processing. The collected data cannot be used or stored for 
            any other purpose than these purposes: Compliance with legal obligations, storage of consent. Consent data 
            (granted consent and revocation of consent) is stored for three years. After this period, the data will be deleted 
            immediately or handed over to the responsible person upon request in the form of a data export. You can manage 
            your consents by clicking on the button <a href="https://www.welthungerhilfe.org/privacy/#c24231" target="_blank">at the top of the page</a>.</p>

        <h2>9. Notice to Parents and Legal Guardians</h2>
    
        <p>We do not allow children under the age of 18 years to register on the <i>AgriShare</i>. If you are under 18 years 
            of age, we do not want your personal data, and you must not provide it to us. If you are a parent or guardian and 
            believe that your child who is under 18 years of age has provided us with personal data, please contact us to have 
            your child’s data removed.</p>
    
        <h2>10. What if we change this privacy notice or any of our privacy notices?</h2>
    
        <p>We may need to change this notice and our notices. The updates will be posted online. Your continued use of the 
            product or service after the effective date of such changes constitutes your acceptance of such changes. To make 
            your review more convenient, we will post an effective date at the top of the page.</p>
    
        <h2>11. Your Questions or Comments</h2>
    
        <p>You are welcome to contact our data protection officer with any questions, suggestions or complaints regarding 
            our Privacy Notice. You can reach our data protection officer by email at 
            <a href="mailto:datenschutz@welthungerhilfe.de" class="s6" target="_blank">datenschutz@welthungerhilfe.de</a> 
            or by post at our postal address with the addition of “To the data protection officer”. If you wish to 
            receive information on your data, change or delete it, please contact us.</p>
    </div>

    <div id="shona" ng-show="language === 'shona'">
        <h1>Chiziviso Chezvekuchengetedzeka Kweruzivo KuAgriShare</h1>
        <p>Isu, ve Zimbabwe Welthungerhilfe, tinokoshesa kuchengetedzeka kweruzivo rwunechekuita newe sedungamunhu zvakanyanyisa kwazvo. Ruzivo rwese rwunechekuita newe ipfimbi yedu yatisina kana mumwe watinoudza, uye tinorwuchengetedza rwakavanzika tichitevedzera mitemo yezvekuchengetedzwa kweruzivo pamwechete neChiziviso icho chino.</p>

        <h2>1. General Information</h2>
        <p>Muchiziviso chino, tinotsanangura mhando, hudzamu nechinangwa chekushandiswa kweruzivo rwunechekuita nedungamunhu muApplication yedu yeAgriShare, madandemutande anodyidzana nayo, mibato yakasiyanasiyana, pamwe nezvakagukuchirwa (zvinova zvatirikungoshandisira izwi rekuti &quot;AgriShare&quot; zvese muchiziviso chino). Chiziviso ichi chinongoshanda zvisinei nekuti munhu ashandisa kero ipi yehindaneti, mashandiro emudziyo wekubata nawo masaisai, nzira dzekukutana nadzo pamasaisai, kana kuti mhando yemudziyo washandiswa (ingava kombiyuta kana kuti nharembozha) kubata nawo masaisai ezviwanikwa zvedu.</p>
        <p>Chiziviso chezvekuchengetedzwa kweruzivo ichi chinobata kushandiswa kwese kunoitwa ruzivo neve:<br />
            Deutsche Welthungerhilfe e. V.<br />
            Friedrich-Ebert-Str. 1<br />
            53173 Bonn<br />
            Germany</p>
        <p>Mashoko akafanana nekuti &quot;ruzivo rwunechekuita nedungamunhu&quot; kana &quot;kushandiswa&quot; kwarwo kunotsanangurwa maringe neChikamu chechina muMutemo wezvekuchengetedzwa kweruzivo, (kana kuti General Data Protection Regulation (GDPR)). Tinongoshandisa ruzivo rwunechekuita nevanhu vanopinda pazvikuva zvedu chete tichicherechedza mitemo yakakodzera yezvekuchengetedzwa kweruzivo. Izvi zvinosanganisira kushandisa ruzivo rwunechekuita nedungamunhu zviripamutemo, zvichienderana nemvumo yemunhu yaanopa achinyatsoziva zvekutarisira (§ 6.1(f) GDPR); maringe nevavariro yedu yemazvirokwazvo (§ 6.1(f) GDPR) kana panechikonzero chekudaro, zvinosanganisira vavariro yekuita ongororo, yekunyatsofambisa zvinhu negwara kwaro, pamwechete nekuita mabasa anounza pundutso zvese nekudziviririka kwezviwanikwa zvedu zvepamasaisai; kana kutiwo kuzadzikisa zvatinotarisirwa kuita nemutemo (§ 6.1(c) GDPR).</p>
        <p>Mugwara rimwero, shoko rekuti &quot;mushandisi&quot; rinoreva ani nani zvake angave anechekuita mukushandiswa kunoitwa ruzivo rwunowanikwa pazvikuva zvedu; izvi zvinosanganisira vanhu vanoshanyira dandemutande redu pahindaneti pamwechete nevanoshandisa application yedu yeAgrishare. Mavara anoshandiswa apa, sekuti &quot;mushandisi&quot;, anogona kureva munhurume kana munhukadzi.</p>

        <h2>2. Chii chinonzi &quot;ruzivo rwunechekuita newe?&quot;</h2>
        <p>Kana tichiti &quot;ruzivo rwunechekuita newe&quot; tinenge tichireva mhando kana ipi zvayo yeruzivo rwunogona kutondeka kuti ndiwani, zvinosanganisira:</p>
        <ul>
            <li>Zita rako, zuva rawakazvarwa, kuti urimunhui uye unosarudza kushandisa mutauro upi</li>
            <li>zvinechekuita nemari, kusanganisira akawundi yako yekubhangi, nhamba dzako dzenhare</li>
            <li>nhamba dzako dzechitupa, kana dzimwewo nhamba dzinoratidza kuti ndiwani, kusanganisira nhamba dzekuve nhengo kana dzezvirongwa zvakatsarukana</li>
            <li>maonero ako kana kuti mafungire ako kana zvaunonyanyofarira pamusoro pemashandisiro aunoita zviwanikwa nerubatsiro rwedu, izvo zvatinowana patinoita tsvakurudzo dzedu dzekushambadzira pamwe nekutsvaka miono.</li>
            <li>nhamba dzako dzenhare dzawakashandisa kunyoresa kuti uwane zviwanikwa zvedu pamwe nerubetsero rwatinopa, kusanganisira manhamba akanyoreswawo nechinangwa chekutumira kana kutumirwa mashoko, kana kushandisa pakutenga, kutumira mameseji uyewo kufona.</li>
            <li>kero yako yetsambagetsi (email) yawakanyoresa kuti unanavire nayo zviwanikwa zvedu pamwe nerubatsiro rwatinopa, kusanganisirawo dzimwe kero dzetsambagetsi dzatinadzo mumagwaro edu, dzatinoshandisa kutumira tsamba dzekushambadza nekutengesa zvatinazvo panguva dzakatsarukana.</li>
            <li>pawakanyora kana kusayina uchitipa mvumo yekuti titore ruzivo rwose rwarehwa pamusoro,</li>
        </ul>
        <p>Tinochengetedzawo rumwe ruzivo rwakanangana newe, kunyangwe rwusingataridzi kuti ndiwani zviripachena asi rwuchigona kuzongoshandiswa neumwewo mutowo wekuratidza kuti unombori ani, nenzira imweyo yatinochengetedza nayo ruzivo rwunokunongedzera zviripachena. Semuenzaniso,</p>
        <ul>
            <li>kero inotondekera mudziyo wako unobata masaisai ehindaneti (IP address)</li>
            <li>nhamba dzinotsaura mudziyo wako panemimwe inowanikwa pasi rose (IMEI)</li>
            <li>nhamba dzako dzenhare kana kuti simu kadhi</li>
            <li>minongedzo yemasaisai inoratidza panemudziyo wako nguva imwe neimwe.</li>
        </ul>

        <h2>3. Tinounganidza sei ruzivo rwakanangana newe?</h2>
        <p></p>
        <ul>
            <li>paunovhura akawundi yako kuburikidza nekutipa zita rako, zuva rawakazvarwa, kwauri, nerwumwewo ruzivo rwakanangana newe rwakadai senhamba dzako dzechitupa, nhamba dzako dzenhare, kana kero dzako dzetsambagetsi</li>
            <li>paunongovhurira App yeAgriShare, nekuti inobva yatongotanga kuratidza pauri panguva iyoyo, zvisinei nekuti urikuita zvekutenga here kana kuti kwete</li>
            <li>pegapega panotsvagwa minongedzo yepauri neApp kuitira kuti igone kukusarudzira zviwanikwa zvinoenderana nepauri panguva iyoyo</li>
            <li>paunotsvaga ruzivo rwurimaringe nezvemichina yekushandisa inohayiswa, paunonyora michina yaurikuhayisa, paunotumira kana kuwana zviziviso zvakasiyanasiyana</li>
            <li>paunobhukisa kuwana rubatsiro nezviwanikwa zvakatsarukana zvirimuApp</li>
            <li>paunonyora ruzivo rwezvemari yako yepafoni, zvinechekuita nebhangi rako kana kuti neakawundi yako yekubhangi.</li>
            <li>paunotenga uchishandisa AgriShare, tinobva taziva nhamba yako yemari yemufoni nemashandisiro ese aunoita mari ipapo</li>
            <li>pegapega paunoshandisa nzvimbo dzedu dzekukutana nevamwe pamasaisai, dandemutande nemaApplication edu, kunyangwe usiri mutengi wedu kana kuti wavhurisa akawundi</li>
        </ul>
        <p>Ruzivo rwatinochengeta rwunosanganisira zvikamu zvinotevera zveruzivo:</p>
        <ul>
            <li><b>Akawundi.</b> Ruzivo rwunechekuita neakawundi yako yeAgriShare, rwunosanganisira kero yako yetsambagetsi, kwauri, mudziyo wauri kushandisa, pamwe nekuti akawundi yako yakamira sei.</li>
            <li><b>Mudziyo waurikushandisa.</b> Ruzivo rwunechekuita nemudziyo waunoshandisa paAgriShare, rwunosanganisira serial number, pegapega panenge panemudziyo iwoyo, uyewo mhando nezita remudziyo wacho</li>
            <li><b>Kutenga.</b> Ruzivo rwunechekuita nekutenga kwese kwaunoita paAgriShare pamwechete nemamwewo mashandisiro aunoita mari uripaAgriShare.</li>
            <li><b>Mashandisiro.</b> Ruzivo rwunechekuita nemashandisiro aunoita App yeAgriShare nerwunobva paApp/padandemutande raurikuishandisira, kusanganisira nhoroondo yepese pawapinda; zvese zvawatsvaga; kudonha kwemasaisai kusina kutarisirwa, mashandiro ayo nemamwewo matambudziko awasangana nawo pakuishandisa; nemamwe mashandisiro ose zvawo aungaite App yacho.</li>
            <li><b>Maonero:</b> Tsvakiridzo dzemaonero ako, mhinduro dzako kumibvunzo yaunopiwa pamwechete nerwumwewo ruzivo rwaunongopa wega maringe nekushanda kwezvikuva zvedu zvepamasaisai</li>
            <li><b>Kushambadza pamasaisai:</b> kuburikidza nezvirongwa zvedu zvekushambadzira zvinotengeswa, zvikuva zvekutsvagira zvaunoda pamasaisai, mapeji epadandemutande, tsambagetsi. Ruzivo urwu rwunounganidzwa neshambadzo zvichienderana nepese pawabaya kuti uone zvizere, kusanganisira mhando yemudziyo wawashandisa</li>
            <li><b>Mashandisirwo enzvimbo dzekukutana pamasaisai:</b> Kuburikidza nekuongorora rudhende rwevanhu vanoteverera mapeji edu enzvimbo dzekukutana pamasaisai, tichatarira kuti unobatika sei pamasaisai, kuitira kuti tigone kuwana nzwisiso yakakwana yekuti vanhu vanotitsigira vanhu vakaita sei – zvisinei nekuti urimutengi wedu here kana kuti kwete.</li>
        </ul>

        <h2>4. Ruzivo rwunechekuita newe tinorwuitisei kana tarwuunganidza?</h2>
        <p>Tinongoshandisa ruzivo urwu zvatarwuunganidzira chete, sezvo zvirizvo zvatinosungirwa kutevedza nemitemo yezvekuchengetedzwa kweruzivo kuitira kuti;</p>
        <ul>
            <li>tigone kuwanisa rubatsiro rwatinopa pazvikuva zvedu, nepamaapplication</li>
            <li>tigone kuvandudza mashandiro ezvikuva zvedu zvemasaisai pamwechete nerubatsiro rwatinokuwanisai.</li>
            <li>tigone kukubatanidza nevanogona kukubatsira panezvaurikutsvaga, kana vamwewo vavanodyidzana navo vangangokuyamura</li>
            <li>tigone kugovera mishumo yose irimaringe nemashandiro eAgriShare, uyewo nezvinangwa zvekushambadza zviwanikwa zveAgriShare asi tisingabuditse ruzivo rwunechekuita newe pamhene</li>
            <li>tigone kutevedza mitemo yose yenyika yakakodzera inotisungira kuunganidza rwumwe ruzivo rwunechekuita nedungamunhu, yakadai semutemo wezvemabhangi (Banking Act), mirawu inofambidzana nawo, nemirayiridzo yekuziva mutengi wako (KYC),</li>
            <li>tigone kutora matanho akakodzera echengetedzo nekudzivirira hukopokopo, dzimwe mhosva dzinoparwa kuburikidza nemasaisai, dzakafanana nekuba zita remunhu kwakunyepedzera kuva iye,</li>
            <li>tigone kuvandudza kudyidzana kwedu newe semutengi wedu, paruyamuro rwese rwatinopa nezviwanikwa zvatinotengesa, uyewo kuti tigone kuvandudza ruyamuro nezviwanikwa zvimwezvo, kuitira kuti tikugutse panezvese zvaunotsvaga kwatiri,</li>
            <li>tikwanise kuongorora mashandiro atinoita nekugadzirisa misikidzo dzekunatsurudza mashandiro edu.</li>
        </ul>

        <h2>5. Tinopa vamwe ruzivo rwunechekuita newe kana zvadii?</h2>
        <p>Ruzivo rwese rwunobva pakushandiswa kweApp yedu rwunogona kuonekwa nevekambani yatinoshandidzana nayo yeCommunity Technology Development Organisation (CTDO).</p>
        <p>Tinoshandisawo mamwe makambani akazvimiririra kuti afambise basa redu pachinzvimbo chedu, uye izvi zvinoitwa pachitevedzwa zvisungo zveChikamu 28 cheMutemo weGDPR. Takanyorerana zvibvumirano pasi zvekuti ruzivo rwese rwunogovanwa saizvi rwungoshandiswa maringe nezvinangwa zvarwatorerwa chete nemakambani atinoshandidzana nawo kuitira kuti ruzivo rwunechekuita newe rwusazoshandiswe zvimwe zvinhu kana kupihwa vamwewo vanhu zvisina tsarukano. Pamusoro pezvo, tinematanho atinotora pakufambiswa kwemabasa nemaitiro esangano redu kuti tichengetedze zvakwana ruzivo rwunechekuita nedungamunhu.</p>
        <p>Izvi zvinosanganisira masangano anotevera nemamwewo asina kudomwa:</p>
        <ul>
            <li>VeC2 Digital, Harare, Zimbabwe: Vanogadzira dandemutande redu nekuona kuti ririkushanda zvakanaka nguva dzose</li>
            <li>VeZimbabwe Farmers Union, 5 Van Praagh Avenue, Milton Park. Harare</li>
            <li>Hurongwa hwekushandiswa kwezviredzo kana kuti makuki, hunofambisa gwara rekupa mvumo kumakuki anoredza ruzivo</li>
            <li>VeGoogle Inc., 1600 Amphitheatre Parkway, Mountain View, CA 94043, USA: Vanoongoroa zviripadandemutande redu nekushambadza zviwanikwa zvedu zvose.</li>
        </ul>
        <p>Maringe nekushandiswa kwezviwanikwa, zvekushandisa, kana zvimwewo zvinhu zvakanangana neChiziviso Chekuchengetedzeka Kweruzivo chino nemamwe masangano anoshandidzana nemasangano atinoshanda pamwechete nawo (vatichati muno &quot;Masangano Etatu&quot;) akanyoreswa kunyika yetatu, ngazvitongotorwa sekuti ruzivo urwu rwurikutoendeswawo kunyika yesangano retatu iri. Nyika dzetatu inyika dzisingasungwi zvakajeka nemutemo weGDPR – tichitarisa zvikamu zvakasiyanasiyana, uye nyika idzi dzinosanganisira dzakadai sedzisiri muMubatanadzwa weNyika dzemuYuropu (European Union) nedzirikunze kweDunhu Rezvekutengeserana Kwenyika dzemuYuropu (European Economic Area). Ruzivo rwunogona kuendeswa kunyika dzetatu, hunge patorwa matanho akakodzera ekurwuchengetedza, painemvumo yemushandisi, uyewo mimwe mirawu yakafanira inobvumidza kudaro.</p>
        <p>Patinosungirwa nemutemo, pakaita sepakuferefetwa kwedzimhosva, tinotarisirwa kupa ruzivo rwunosanganisira rwunechekuita nedungamunhu kumasangano emutemo anenge achiferefeta nyaya dzacho mushure mekunge tagudzikana kuti anemvumo yakakwana sezvinotarisirwa pamutemo, yekuti awane ruzivo rwacho irworwo.</p>

        <h2>6. Tinochengeta pai uye tichichengetedza sei ruzivo rwunechekuita newe sedungamunhu?</h2>
        <p>Takazvipira kuchengetedza ruzivo rwunechekuita newe sedungamunhu kana rwuchinge rwava mumaoko edu. Tinotora matanho ekuchengetedza ruzivo pazvidanho zvakasiyana zvinosanganisira midziyo inobatika, hwaro hwebasa pamwe nemafambisirwo anoitwa basa. Mumatanho ese iwayo atinotora, kurikuti zvaitikawo kuti pave nechinokanganisa kuchengetedzeka kweruzivo rwunechekuita newe, tinokuzivisa kuti uwane kutorawo matanho akafanira ekuzvidzivirira.</p>
        <p>Nesuwo hatitodi kuchengeta ruzivo rwunechekuita newe sedungamunhu kwenguva inopfurikidza patinoda kurwushandisa, saka nekudaro tinongorwuchengeta panguva iyo tinenge tichiri kuita zvatarwutorera chete. Tichinge tangopedza narwo, tinobva tatorwuparadza, kunze kwekuti kana tasungirwa nemutemo kuti timboramba takabatira parwuri. Ruzivo rwunechekuita nedungamunhu rwurimumaakawundi ese akavharwa rwunosungirwa kuchengetwa mutsapi dzezivo kwemakore matanhatu neMutemo wezvekufambiswa kwemashoko nePosvo neDzinhare muZimbabwe (Postal and Telecommunications Act of Zimbabwe); panongopererawo Makore matanhatu iwayo, rwunobva rwatodzimwa zvamuchose mumasevha etsapi dzezivo iwayo.</p>
        <p>Ruzivo rwatinochengeta isu rwunodzimwa rwese patinenge tangopedza narwo zvarwunenge rwakatarirwa kuitiswa, uye kudzimwa kwarwunoitwa hakuna matyorero akunoita mitemo irimaringe nezvenguva yekuchengetwa kweruzivo. Paya pekuti ruzivo rwatadza kudzimwa nekuda kwekuti rwanzi rwumbochengetwa nevemutemo, mashandisirwo arwo nawo atori nemuganhu. Izvi zvinoreva kuti rwunongoiswa parutivi rwurirwega rwosashandisirwa zvimwe zvinangwa. Izvi zvinosanganisira, semuenzaniso, ruzivo rwakachengeterwa zvemabhizimusi kana kuti zvinechekuita nemitero.</p>
        <p>Maringe nezviga zvemutemo, ruzivo rwese rwuripasi peChikamu 257.1 cheMutemo weHGB (German Commercial Code) rwunofanirwa kuchengetwa kwemakore matanhatu (masitetimendi egore anotsanangura mashandisirwo emari, mavhocha, nezvimwe zvakangodarodaro), uye ruzivo rwuripasi peChikamu 257.1 cheMutemo weAO (German Revenue Code) rwunofanirwa kuchengetwa kwemakore anosvika gumi (mabhuku, zvinyorwa, magwaro anotsanangara mabatirwo emabasa, mavhocha, matsamba anechekuita nezvemitero, nezvimwewo zvakangodarodaro).</p>
        <p>AgriShare inogona kuchengeta rwuzivo rwusina kunangana nedungamunhu uye rwusingagoni kunongedza mwene kwenguva yakati rebei nechinangwa chekuburitsa mishumo inodikanwa nepazvinosungirwa nemutemo kuti ruzivo rwemhando iyoyo rwuchengetwe kwenguva inenge yakatarwa.</p>
        <p>Ruzivo rwunechekuita nemi tinorwuchengetera mumasevha arimuno muZimbabwe, uye nemumasevha arikuFrankfurt, kuGermany, kuitira kuti pakava nezvinoitika kumasevha ekuno, rwuzivo rwacho harwuzorasiki zvachose. Mukutipa ruzivo rwenyu, munenge matotipa mvumo yekuti ruzivo urwu rwuchengetwe nemutowo iwoyo kumasevha arehwa. Tinotevedzera mitemo yose inoshanda panhau idzi, uye hatitsauki mugwara rekuzvipira kwedu sekutsanangurwa kwazvakaitwa muChiziviso Chekuchengetedzeka kweRuzivo chino.</p>

        <h2>7. Kodzero dzako ndedzipi?</h2>
        <p>Kana uchinge wangoisa chikumbiro chako nekutigutsa kuti ndiwe ngana, tinofara kwazvo kukunyorera tichikuzivisa maringe nemutemo wakakodzera kuti paneruzivo rwunechekuita newe sedungamunhu here rwashandiswa. Kurikuti rwuripo, unekodzero yekubudisirwa pachena kuti nderwupi ruzivo rwacho pamwechete neruzivo rwakatsanangurwa pasi peChikamu 15 cheMutemo weGDPR. Kana ruzivo rwunechekuita newe urwu rwawanikwa rwakakanganisika, unekodzero yekuti ruzivo rwese rwakaresveka ngarwugadziriswe pariporipotyo, nekuti ruzivo rwusina kunyorwa zvizere rwuzadziswe zvakaperera (§ 16 GDPR).</p>
        <p>Unekodzero yekuti ruzivo rwunechekuita newe sedungamunhu rwudzimwe pariporipotyo nekuda kwechimwe chezvikonzero zvakatarwa pasi peChikamu 17 cheMutemo we GDPR, semuenzaniso, kana ruzivo rwacho rwusisingadikwe kuzadzisa chinangwa charwakambotorerwa (kodzero yekudzimwa).</p>
        <p>Unekodzero yekuisisa muganhu mukushandiswa kweruzivo rwunechekuita newe kana chimwe chezviga zvakatarwa pasi peChikamu 18 cheMutemo we GDPR chichishanda ipapo; semuenzaniso, kana waramba kuti ruzivo rwunechekuita newe rwushandiswe, izvi zvichatevedzwa kwenguva yese yazvichatora kuti muongorori apedze kupepeta nyaya yacho.</p>
        <p>Unekodzero yekuramba chero nguva kuti ruzivo rwunechekuita newe sedungamunhu rwushandiswe nekuda kwezvikonzero zvako zvauchapa. Kambani yedu inobva yaregera kuramba ichishandisa ruzivo irworwo rwunenge rwarehwa, kunze kwekuti pachinge pawanikwa chikonzero chakasimba chekuti tienderere mberi nekurwushandisa, kana kuti hunge kurwushandisa kuchidikanwa pakutsvaga humbowo, kuzadzikisa kana kuti kuzvidzivirira pamutemo (§ 21 GDPR).</p>
        <p>Nenzira isingazodzvanyiriri kana kukonesa kufambiswa zvakanaka kwebasa kana mutemo, unekodzero yekuisa chichemo kunhungamiri dzebasa kana unechikonzero chekutenda kuti mashandisirwo arikuitwa ruzivo rwunechekuita newe sedungamunhu arikutyora zvisungo zveMutemo weGDPR (§ 77 GDPR). Unogona kushandisa kodzero iyi kuburikidza nenhungamiri dzakakodzera dzirimunyika yaunogara, kwaunoshandira, kana kuti dzekunyika yaunofungira kuti ndiko kwakatyorerwa mutemo wacho. Kana kuri kuNorth Rhine–Westphalia, munhu anotevera ndiye anotonga nyaya dzacho:</p>
        <p>The data protection officer for North Rhine–Westphalia (LDI)<br />
            Kavalleriestrasse 2 - 4<br />
            40213 Düsseldorf Germany</p>

        <h2>8. Zviredzo (makuki) Nekuerwa kwenanaviro</h2>
        <p>Zviredzo (kana kuti makuki) ruzivo rwunobva kumasevha edandemutande redu kana kuti kumasevha emasangano etatu rwuchinonamatira pabhurawuza yaurikushandisa kupinda pahindaneti, rwochengetwapo nechinangwa chekuzotorwa pava paya. Makuki aya aripamhando dzakatsarukana, dzinosanganisira zvimafaira zvidoko kana dzimwewo nzira dzekuchengeta ruzivo. Maringe neChiziviso Chekuchengetedzwa kweruzivo chino, uchange uchiziviswa pese panoshandiswa makuki aya kuera pese paananavira, asi pasingashandiswi mazita emazvirokwazvo. Kana usingadi hako kuti makuki aya achengetwe pakombiyuta pako, unokumbirwa kuti usarudze mhiko inorambidza kuchengetwa kwemakuki kumasettings ebhurawuza yako. Makuki ese akachengetwa pakombiyuta pako anogona kudzimwa kumasettings ebhurawuza yako zvekare. Kubvisa makuki uku kunogona kuganhura mashandiro anoita dandemutande pakombiyuta yako.</p>
        <p><a href="https://support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies" target="_blank">Internet Explorer:
            support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies</a></p>
    
        <p><a href="https://support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences" target="_blank">Firefox:
            support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences</a></p>
    
        <p><a href="https://support.google.com/chrome/answer/95647?hl=en-GB" target="_blank">Google Chrome:
            support.google.com/chrome/answer/95647?hl=en-GB</a></p>
    
        <p><a href="https://support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac" target="_blank">Safari:
            support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac</a></p>
    
        <p><a href="http://help.opera.com/Linux/12.10/en/cookies.html" target="_blank">Opera:
            help.opera.com/Linux/12.10/en/cookies.html</a></p>

        <p>Unokwanisawo zvekare kumisa kushandiswa kwemakuki mukuera pose paanonanavira nemukushambadzirwa kwezviwanikwa zvakasiyanasiyana kuburikidza nepeji rekumiswa kwezveshambadziro panetiweki, rinonzi Network Advertising Initiative’s deactivation page, rinowanikwa pakero inoti (<a href="http://optout.networkadvertising.org/" target="_blank">http://optout.networkadvertising.org/</a>) nepadandemutande rekuAmerika rinobatwa nekeri inoti (<a href="http://www.aboutads.info/choices" target="_blank">http://www.aboutads.info/choices</a>) uye dandemutande rekuYuropu rinobatika pakero inoti (<a href="http://www.youronlinechoices.com/uk/your-ad-choices/" target="_blank">http://www.youronlinechoices.com/uk/your-ad-choices/</a>). Tinoshandisawo zvekare hwaro hwekupa mvumo hunokoshesa zvido zvemushandisi pazvikuva zvedu, huchikotsverwa neveUsercentrics GmbH. Hwaro uhwu inzira yekupa nayo mvumo kuburikidza nekudoma zvinangwa zvakatsarukana zvekutorwa nekushandiswa kweruzivo. Naizvozvo, ruzivo rwunenge rwatorwa harwugoni kuchengeterwa kana chipi zvacho chikonzero chisiri chinenge chakatarwa: Kutevedza zvinodikanwa nemirawu yemutemo, kuchengetwa kwemvumo. Ruzivo rwunechekuita nemvumo (mvumo yapihwa nemvumo yanyimwa) rwunochengetwa kwemakore matatu. Panopera nguva iyi, ruzivo urwu rwunobva rwadzimwa zvamuchose kana kuti kupihwa kumunhu akakodzera mushure mekunge paiswa chikumbiro chakafanana nechekutumirwa kweruzivo (data export). Unogona kuronga mvumo dzako kuburikidza nekubaya bhatani <a href="https://www.welthungerhilfe.org/privacy/#c24231" target="_blank">riripamusoro pepeji rino</a>.</p>

        <h2>9. Chiziviso Kuvabereki Nevachengeti Vevana</h2>
        <p>Hatitenderi vana varipasi pemakore gumi nemasere (18) kuti vanyorese paAgriShare. Kana uripasi pemakore 18 ekuzvarwa, hatidi ruzivo rwunechekuita newe sedungamunhu, uye hautombofanirwi kutipa herwo. Kana urimubereki kana kuti muchengeti wemwana zviripamutemo, uye uchitenda kuti mwana wako aripasi pemakore 18 ekuzvarwa atipa ruzivo rwunechekuita naye sedungamunhu, tapota tibate tidzime ruzivo rwunechekuita nemwana wako.</p>

        <h2>10. Ko kana tikashandura chiziviso chekuchengetedzwa kweruzivo chino kana zvimwewo zvisiviso zvedu zvinechekuita nekuchengetedzwa kweruzivo?</h2>
        <p>Zvinogona kuitika kuti pave nechikonzero chekuti tichinje chiziviso chino nezvimwe zviziviso zvedu. Shanduko dzose dzichaitika dzichaiswa pazvikuva zvedu zvemasaisai. Kuramba kwako uchishandisa chikuva kana kuti zviwanikwa zvedu mushure meshanduko iyoyo kunoreva kuti unoigamuchira shanduko yacho. Kuitira kuti ugone kunyatsoziva zvirikuitika nyorenyore, tichanyora musi uchatanga kushanda shanduko imwe neimwe pamusoro pepeji.</p>

        <h2>11.	Mibvunzo yako kana zvimwewo zvaungada kutaura</h2>
        <p>Wakasununguka kubata mukuru wedu anoona nezvekuchengetedzwa kweruzivo kana unemibvunzo, mazano aungade kupanga, kana zvichemo zvaungade kusvitsa maererano neChiziviso chedu Chezvekuchengetedzwa Kweruzivo. Mukuru wedu anoona nezvekuchengetedzwa kweruzivo anobatika netsambagetsi pakero inoti <a href="mailto:datenschutz@welthungerhilfe.de" class="s6" target="_blank">datenschutz@welthungerhilfe.de</a> kana kuti netsamba pakero yedu yeposvo, iyo yaunofanirwa kunyora pamusoro kuti &quot;Kuna Mukuru wezvekuchengetedzwa kweruzivo&quot; (kana kuti &quot;To the data protection officer&quot;). Kana ungadewo kuziva nezveruzivo rwunechekuita newe sedungamunhu rwatinarwo, kurwushandura kana kurwudzima, tapota tibatewo zvekare.</p>
    </div>

    <div id="ndebele" ng-show="language === 'ndebele'">
        <h1>ISaziso se-AgriShare Esiphathelane LeMfihlo YoMuntu</h1>
        <p>Thina, njengabe Welthungerhilfe eZimbabwe, siqakathekisa ukugcinakala kuyimfihlo, kolwazi oluphathelane lomuntu siqu sakhe kakhulukazi. Ulwazi oluphathelane lawe siqu sakho siluphatha njengemfihlo njalo silandela iziqondiso zemithetho ehola ukuvikelwa kolwazi kanye lasonalesi Saziso esiphathelane leMfihlo yoMuntu.</p>

        <h2>1. Ulwazi Jikelele</h2>
        <p>Kulesi Saziso esiphathelane leMfihlo yoMuntu, sichaza umhlobo, kanye lesizatho sokusetshenziswa kolwazi oluphathelane lomuntu siqu sakhe ku-application ye-AgriShare, amawebhusayithi (website) adlelana layo, imisebenzi yayo, kanye lemininingwane eyiqhuketheyo (esikubiza lapha ngokuthi &quot;Agrishare&quot; konke). ISaziso esiphathelane leMfihlo yoMuntu lesi siyasebenza kungakhathalekile ukuthi yisiphi isizinda sobulembu esisetshenziswa ngumuntu, uhlelo lwekhompiyutha (computer), inkundla yokuxhumana, noma impahla (njengomzekeliso, ikhompiyutha kumbe umakhalekhukhwini) esetshenziswayo ekufinyeleleni insiza zethu zebulembini. </p>
        <p>ISaziso esiphathelane leMfihlo yoMuntu lesi silawula ukuphathwa kolwazi ngabe:<br />
            Deutsche Welthungerhilfe e. V.<br />
            Friedrich-Ebert-Str. 1<br />
            53173 Bonn<br />
            Germany</p>

        <p>Amagama afana lathi &quot;ulwazi oluphathelane lomunt siqu sakhe&quot; kumbe &quot;ukusetshenziswa&quot; kwalo achazwe njengokubalulwe kuSigaba 4 soMthetho Jikelele oQondisa ukuVikelwa koLwazi, owaziwa ngokuthi yiGeneral Data Protection Regulation (GDPR). Ulwazi oluphathelane labasebenzisi benkundla zethu silusebenzisa kuphela ngendlela ezilandela iziqondiso ezifaneleyo zokuvikelwa kolwazi. Lokhu kugoqela ukusebenzisa ulwazi oluphathelane labasebenzisa isiqu sabo ngokusemthethweni, kusiya ngemvumo eyabe iphiwe yibo abasebenzisi besazi ngokupheleleyo ukuthi kwenzakalani (§ 6.1(f) GDPR); malunga lenhloso zethu eziqotho (§ 6.1(f) GDPR) nxa kudingakala, sitsho phela inhloso zokucubungula, ukulungisisa yonke into ukuze ihambe ngendlela, kanye lokuqhutshwa kwebhizimusi ndawonye lokuvikeleka kwenkundla zethu ezisebenza nge-intanethi/ ebulembini; noma ukuze sigcwalisise imilandu yethu ngokomthetho (§ 6.1(c) GDPR).</p>
        <p>Kumkhandlo munye, igama elithi &quot;umsebenzisi&quot; litsho wonke amaqembu abantu abathintwa yikuphathwa okwenziwa ulwazi enkundleni zethu; lokhu kugoqela abantu abethekelela iwebhusayithi yethu ebulenjini kanye labantu abasebenzisa i-application ye-AgriShare. Amagama asetshenziswa lapha, njengokuthi &quot;umsebenzisi&quot;, angakhomba loba yibuphi ubulili.</p>

        <h2>2. Kuyini &quot;ulwazi oluphathelane lomuntu siqu sakhe&quot;?</h2>
        <p>Nxa sisithi &quot;ulwazi oluphathelane lomuntu siqu sakhe&quot; siyabe sisitsho loba yiluphi ulwazi kumbe imininingwane ekukhomba ngqo ukuthi ungubani, olufana lokulandelayo:</p>
        <ul>
            <li>ibizo lakho elisesithupheni, usuku lwakho lokuzalwa, ubulili bakho kanye lolimi ofuna ukulusebenzisa</li>
            <li>ulwazi olulokokwenza lokusebenzisa kwakho imali, okugoqela inombolo  yakho yasebhanga, inombolo zakho zocingo</li>
            <li>inombolo zakho zesithupha, noma yiphi enye inombolo eqondane lawe eveza ukuthi ungubani, okubalisela inombolo zokuba lilunga kumbe ezokungena enhlelweni ezithize zokukhangisa</li>
            <li>imibono yakho kumbe indlela obukeza ngayo noma okhetha ukusebenzisa ngayo izinto kanye lensiza zethu, esikuthola ngesikhathi siqhuba izicwaningo zemikhankaso lezokudinga imibono </li>
            <li>inombolo zakho zocingo owazibhalisa ukuze uthole izinto kanye lensiza zethu ngazo, kugoqela lenombolo ezingabe zibhaliselwe ukuthumela kumbe ukuthola ngazo imibiko, ukuthenga lokuthengisa, insiza zokuhamba kwemibiko, kanye lokufona.</li>
            <li>ikheli yakho yencwadi zebulenjini (i-email) owayibhalisa ukuze uthole izinto kanye lensiza zethu ngazo, kugoqela laloba yiphi enye ikheli ye-email engabe ibhaliswe eluhlwini lwethu lwamakheli okuthumela izincwadi ezilemibiko yokukhangisa kumbe ukukhankasa noma ezinye izinsiza ezifanayo.</li>
            <li>lapho ongabe wabhala khona kumbe ukusayina usinika imvumo yokuqoqa ulwazi oluqanjwe ngaphezulu,</li>
        </ul>
        <p>Siphatha njalo olunye ulwazi olungakuqondi ngqo ukuthi ungubani, kodwa olungasebesinzeseka ngokulengqondo ekukuvezeni, ngendlela efana laleyo esiphatha ngayo ulwazi oluqondane lawe siqu sakho, njengomzekeliso,</p>
        <ul>
            <li>ikheli ekhomba impahla yakho ongena ngayo ebulenjini be-intanethi (i-IP address)</li>
            <li>inombolo ezibhalisiweyo ezehlukanisa impahla yakho lezinye zonke ezisemhlabeni (ikhodi ye-IMEI)</li>
            <li>inombolo zakho zocingo (isimu khadi)</li>
            <li>inkomba zesethilayithi ezitshengisa lapho okulempahla yakho khona sonke isikhathi</li>
        </ul>

        <h2>3. Sithatha njani ulwazi kuwe?</h2>
        <p>Siqoqa kuphela ulwazi esiludingayo ukuze siqhubeke sithuthukisa i-AgriShare njalonje. Nanzi indlela esiqoqa ngazo ulwazi oluphathelane lomuntu siqu sakhe ngemva kokuthola imvumo yakhe:</p>
        <ul>
            <li>ekuvuleni kwakho i-akhawunti ngokusinikeza ibizo lakho, usuku lwakho lokuzalwa, lalapho okhona, kanye lolunye ulwazi oluqondane lawe qobo lwakho olugoqela inombolo zakho zesithupha, inombolo zakho zocingo, kumbe amakheli e-email</li>
            <li>lapho ovula khona i-App ye-AgriShare, ngoba ihle itshengise ukuthi ungaphi kwendawo, kungakhathalekile ukuthi wenza okuphathelane lokuthenga na loba hatshi</li>
            <li>lapho i-App icela khona ukufinyelela imininingwane yenkomba ezitshengisa lapho okhona ukuze ikutholise ulwazi oluhambelana lendawo okiyo</li>
            <li>lapho odinga khona ulwazi olukhona olutshengisa imitshina engaqatshwa, ufaka lawe imitshina oyiqatshisayo kulolo luhlu, lalapho othumeza kumbe ukuthola khona imibiko</li>
            <li>lapho obhukhisa khona ukuthola insiza kumbe impahla ezithile ezitholakala ku-App</li>
            <li>lapho ofaka khona imininingwane ephathelane lemali yakho yefonini, ibhanga lakho kumbe i-akhawunti yakho.</li>
            <li>lapho othenga kumbe ukuthengisa khona esizindeni se-AgriShare, siyahle sibe kwazi inombolo zakho zocingo ogcina ngalo imali kanye lokusebenzisa kwakho imali okwenza lapho</li>
            <li>lapho osebenzisa khona izizinda zethu zokuxhumana ebulenjini, amawebhusayithi lama-application, kungakhathalekile ukuthi ungumthengi wethu loba hatshi, lanoma uvulise i-akhawunti ngathi. </li>
        </ul>
        <p>Imininingwane esiyigcinayo igoqela izigaba ezilandelayo zolwazi:</p>
        <ul>
            <li>Ulwazi nge-Akhawunti. Imininingwane ye-akhawunti yakho ye-AgriShare, kugoqela ikheli yakho ye-email, lapho okhona, impahla oyibhalise ukuyisebenzisa, kanye lesimo i-akhawunti yakho esikuyo.</li>
            <li>Ulwazi ngeMpahla oyisebenzisayo. Imininingwane ephathelane lempahla yakho ofinyelela ngayo izizinda ze-AgriShare, egoqela inombolo ezayiphiwayo ekulungisweni (i-serial number), inkomba zalapho ekhona sikhathi sonke, kanye lomhlobo kumbe ibizo laleyo mpahla.</li>
            <li>Ulwazi ngokusebezisa kwakho Imali. Imininingwane ephathelane lokuthenga kanye lezinye zonke indlela osebenzisa ngazo imali okwenza usezizindeni ze-AgriShare.</li>
            <li>Ukusebenzisa Izizinda. Imininingwane ephathelane lokusebenzisa kwakho i-App yeAgriShare, levela kulwazi lwe-App/ lwewebhusayithi oyisebenzisela kulo, okugoqela umlando wendawo zonke ongene kizo; izinto zonke oke wazidinga kiyo; izehlakalo zonke zokuwa kwamagagasi okungakhangelelwanga, ukusebenza kwayo kanye leminye imininingwane ekhomba izinkinga ongabe uhlangane lazo ekuyisebenziseni; leminye imininingwane nje ephathelane lokusebenzisa kwakho leso sizinda.</li>
            <li>Imibono Yakho: Izicwaningo eziphathelane lemibono yakho, impendulo zakho kumibuzo oyiphiwayo noma olunye nje ulwazi olunika ngokuzithandela maqondana lokusebenzisa kwakho izizinda zethu.</li>
            <li>Ukukhangisa Ebulenjini: Ngokusebenzisa imikhankaso yethu yokuthengisa, ezizindeni zokudinga izinto ebulenjini, emakhasini obulembu, lasezincwadini ze-email. Ulwazi lolu luqoqwa yizikhangiso kusiya ngalapho hlabe khona ukuze ubone okugcweleyo, ndawonye lemhlobo wempahla oyisebenzisayo.</li>
            <li>Ukusetshenziswa kweNkundla zokuXhumana eBulenjini: Ngokukhangela uluhlu lwabantu abasebenzisa inkundla zokuxhumana ebulenjini abalandela amakhasi ethu, sizaqoqa ulwazi ngabantu bonke oxhumana labo ebulenjini, ukuze sizwisise ngcono abantu esingabaqopha ekuthengiseni izinsiza zethu, kungelani lokuthi ungumthengi wethu noma hatshi.</li>
        </ul>

        <h2>4. Silusebenzisani Ulwazi Oluphathelane Lawe Ngemva Kokuluthola?</h2>
        <p>Ulwazi oluphathelane lawe silusebenzisela izizatho esiluqoqele zona kuphela, ngoba lokhu yikho okufunwa yizimiso zemithetho ebona ngokuvikelwa kolwazi ukuze;</p>
        <ul>
            <li>sanelise ukunikeza izinsiza ezitholakala kuzizinda zethu, lakuma-application</li>
            <li>Sanelise ukuthuthukisa indlela izizinda zethu zamakhompiyutha ezisebenza ngayo kanye lezinsiza esilinika zona</li>
            <li>sanelise ukukuxhumanisa labaniki bezinsiza ozidingayo, kanye labanye abaletha izinsiza ongaxhumani ngqo labo</li>
            <li>sanelise ukusabalalisa imibiko esiyikhiphayo mayelana lokusebenza kwe-AgriShare, kanye lokukhangeza imisebenzi ye-AgriShare singavezanga imininingwane ephathelane lawe</li>
            <li>silandele ngokupheleleyo yonke imithetho yelizweni esibophela ukuthi siqoqe ulwazi oluthile oluphathelane lomuntu siqu sakhe, efana loMthetho wezamaBhanga (iBanking Act), iziqondiso eziphathelane layo, lomgomo othi Azi Umthengi Wakho (i-KYC),</li>
            <li>sithathe amanyathelo okuthi kube lokulondolozeka kanye lokuvikeleka ngenhloso yokwenqabela kanye lokunanzelela inkohliso, eminye imihlobo yobugebengu obenzelwa ebulenjini, efana lokutshontshelwa ibizo ngumuntu abesezenza wena, </li>
            <li>sithuthukise indlela esigcina ngayo ubudlelwano bethu lawe njengomthengi wethu, kuzinsiza zethu lasezintweni zonke esizitholisayo, lokuze njalo sazi ukulungisa kanye lokuthuthukisa lezo zinto ndawonye lezinsiza esizitholisayo, ukuze sigcwalisise izidingo zakho ngendlela ekusuthisayo,</li>
            <li>siqhube izinhlelo zokukhipha imibiko emayelana lendlela esisebenza ngayo kanye lokuphuma lamaqhinga okulawula ibhizimusi lethu.</li>
        </ul>

        <h2>5. Sabelana labanye imininingwane yolwazi oluphathelane lawe nxa sekutheni?</h2>
        <p>Ulwazi oluvela ekusetshenzisweni kwe-App ye-AgriShare luyafinyelelwa langabanye bethu esibambisana labo abenkampani yeCommunity Technology Development Organisation (CTDO).</p>
        <p>Phezu kwalokho, sisebenzisa ezinye inhlanganiso ezizimeleyo ukuthi zinikeze izinsiza esikhundleni sethu, njalo lokhu sikwenza silandela izimiso zeSigaba 28 soMthetho weGDPR. Sabhalelana phansi izivumelwano lenkampani inye ngayinye ephatha imininingwane yolwazi ukuze kube lesiqiniseko sokuthi ulwazi oluphathelane lawe siqu sakho lusetshenziswa kuphela ekufezeni izizatho eziluthathelwe zona, hatshi ukuthi lube selwabelwa abanye abantu mahlayana nje. Sibuye njalo sithathe amanyathelo afaneleyo angokokuqhutshwa komsebenzi langokuphathwa kwenhlanganiso ukuze sivikele imininingwane yolwazi oluphathelane lawe ngokupheleleyo. </p>
        <p>Lokhu kugoqela izinhlanganiso ezilandelayo, lezinye ezingabe zingasaqanjwanga lapha:</p>
        <ul>
            <li>iC2 Digital, Harare, Zimbabwe: Elungisa lokulondoloza iwebhusayithi yethu ebulenjini</li>
            <li>iZimbabwe Farmers Union, 5 Van Praagh Avenue, Milton Park. Harare</li>
            <li>Uhlelo lokuholwa kwamakhukhi alungiselwe izizinda zethu oluhola ukuphiwa kwemvumo kumakhukhi asigolela ulwazi</li>
            <li>iGoogle Inc., 1600 Amphitheatre Parkway, Mountain View, CA 94043, USA: Elondonya iwebhusayithi yethu lokukhangisa okuthengiswa kiyo</li>
        </ul>
        <p>Nxa sikhangele okumunyethweyo, izikhali zokusebenzisa, noma ezinye izinto ezisetshenziswa malunga lengqikithi yalesi Saziso esiphathelane leMfihlo yoMuntu ngezinye inhlanganiso (esizibiza lapha ngokuthi &quot;ziNhlanganiso zeSithathu&quot;) ezibhaliswe elizweni lesithathu, kumele kuthathwe njengokuthi lolo lwazi lusiwa elizweni elibhaliswe khona iNhlanganiso yeSithathu ephathekileyo. Ilizwe lesithathu yilizwe elingathintwa ngqo ngumthetho weGDPR – nxa sikhangele ngezigaba ezehlukeneyo, njalo lokhu kugoqela amazwe angaphandle koMgwamanqa wamaZwe eYurophu (i-European Union) kanye leSifunda sokuThengiselana samaZwe eYurophu (i-European Economic Area). Ulwazi lulakho ukudluliselwa emazweni esithathu nxa kuthethwe amanyathelo afaneleyo okuluvikela ngemva kokuthola imvumo yomsebenzisi noma kulandelwa elinye igunya elingokomthetho.</p>
        <p>Nxa kudingeka emthethweni, njengomzekeliso, ekuphenyweni kwecala lobugebengu, siyabotshelwa ukuthi sinikeze ulwazi olungagoqela imininingwane ephathelane lomuntu siqu sakhe kwabomthetho abenza lolo phenyo, ngemva kokuthola isiqiniseko sokuthi bavumelekile emthethweni ukuthi bafinyelele lolo lwazi.</p>

        <h2>6. Sigcina kanye lokuvikela njani ulwazi oluphathelane lawe siqu sakho?</h2>
        <p>Sizimisele ukulondoloza ulwazi oluphathelane lawe siqu sakho nxa selusezandleni zethu. Sithatha amanyathelo okuvikela ulwazi lwakho emabangeni agoqela impahla ezibambekayo, umongo webhizimusi kanye lokuqhutshwa kwemisebenzi. Loba nje sisenza imizamo yonke le, nxa kungenzeka ukuthi ukuvikeleka kolwazi esilugcinileyo kuphambaniseke thize, siyahle sikubikele ukuze uthathe amanyathelo afaneleyo okuzivikela.</p>
        <p>Lathi sinje kasifuni kugcina ulwazi oluphathelane lawe siqu sakho okwesikhathi esedlula leso esisidingayo ukulusebenzisa ngaso, ngakho silugcina okwesikhathi silubenzisa lokho esiluthathele khona kuphela. Nxa singasaludingi, sihle sithathe amanyathelo okulutshabalalisa, ngaphandle nxa siphoqwa ngumthetho ukuthi sike siqhubeke silugcinile okwesikhathi esithile. Ulwazi oluphathelane lomuntu siqu sakhe olukuma-akhawunti angasasebenziyo lugcinwa eziphaleni zolwazi ezingasetshenziswa nsuku zonke okweminyaka eyisithupha malunga loMthetho wezePoso/ amakheli leNcingo eZimbabwe (iPostal and Telecommunications Act of Zimbabwe); leyo minyaka eyisithupha iphela kuphela, lonke lolo lwazi luyahle lucitshwe lonke kumaseva ethu agcina ulwazi.</p>
        <p>Ulwazi lonke esilugcinileyo luhle lucitshwe nxa lokho esiluthathele khona kungasadingakali, langokuba ukucitshwa kwalo kungephuli mthetho ophathelane lokugcinwa kolwazi. Nxa kuyikuthi kube lolwazi esingasalucitshanga ngenxa yokuthi ludingwa ngokomthetho, ukusetshenziswa kwalo kunciphile kakhulu. Lokhu kutsho ukuthi ulwazi lwakhona lufakwe eceleni njalo kaluyi kusetshenziswa enye into. Lokhu kwenzakala, njengomzekeliso, kulwazi lomsebenzisi olumele lugcinelwe isizatho sebhizimusi noma okulokokwenza lemithelo.</p>
        <p>Malunga leziqondiso zomthetho, ulwazi olungaphansi kweSigaba 257.1 soMthetho we HGB (iGerman Commercial Code) kumele lugcinwe okweminyaka eyisithupha (izitatimende zomnyaka ezokusetshenziswa kwemali, amavotsha, njalonjalo), kuthi ulwazi olungaphansi kweSigaba 147.1 soMthetho we-AO (iGerman Revenue Code) kumele lugcinwe okweminyaka elitshumi (amabhuku, imibhalo, imibiko emayelana lokuphathwa kwemisebenzi, amavotsha, izincwadi eziphathelane lemithelo, njalonjalo).</p>
        <p>I-AgriShare ilakho ukugcina ulwazi olungaphathelananga lomuntu siqu sakhe, njalo olungeke lusetshenziswe ukuqonda umuntu othile, okwesikhathi eside kulalalapho ukuze lusetshenziswe ekukhipheni imibiko kanye lalapho umthetho uphoqela ukuthi luqhubeke lugciniwe.</p>
        <p>Imininingwane yakho siyigcina kumaseva ethu aseZimbabwe, sibuye silugcine njalo kwamanye amaseva aseFrankfurt, kweleGermany, ukuze nxa kungaba khona okwenzakalayo kumaseva alapha, ulwazi lwakhona kalusoze lusilahlekele okukanompelo.  Ngokusinika ulwazi lwakho, uyavumelana lokuhanjiswa kwalo ngaloluhlobo. Silandela imithetho yonke efaneleyo njalo sizagcina zonke izithembiso esizinikela kizo kulesi Saziso esiphathelane lemfihlo yomuntu.</p>

        <h2>7. Yiwaphi amalungelo akho?</h2>
        <p>Ngemva kokuthola isicelo kanye lokusuthiseka ukuthi nguwe uzibani, siyajabula ukukubhalela sikubikela malunga lomthetho ofaneleyo ukuthi kulemininingwane yolwazi oluthile oluqondane lawe esiyisebenzisileyo. Nxa kunjalo-ke, ulelungelo lokwazi leyo mininingwane kanye lolwazi olubalulwe ngokugcweleyo kuSigaba 15 soMthetho weGDPR.</p>
        <p>Nxa kungatholakala ukuthi ulwazi oluqondane lawe lolo kaluqondanga, ulelungelo lokuphoqa ukuthi ulwazi oluqondane lawe siqu sakho oluphanjanisiweyo lulungisiswe khonaphokhonapho kumbe lufakwe ngokupheleleyo nxa lusilele (§ 16 GDPR). Ulelungelo lokuphoqela ukuthi ulwazi oluqondane lawe siqu sakho lucitshwe khonaphokhonapho nxa esinye sezizatho eziqanjwe kuSigaba 17 soMthetho weGDPR sisebenza, njengomzekeliso, nxa ulwazi lwakhona lungasadingakali ukufeza isizatho obeluthathelwe sona (ilungelo lokucitshwa).</p>
        <p>Ulelungelo njalo lokunciphisa ukusetshenziswa okungenziwa ulwazi oluqondane lawe nxa esinye sezizatho eziqanjwe kuSigaba 18 soMthetho weGDPR sisebenza; njengomzekeliso, nxa ulandule ukuthi lusetshenziswe, lokhu kuzasebenza okwesikhathi esizathathwa ngumlawuli ehlola udaba lwakhona.</p>
        <p>Ulelungelo lokulandula ngaloba yisiphi isikhathi ukuthi ulwazi oluqondane lawe lusetshenziswe ngenxa yezizatho ezisukela esimeni okiso. Inhlanganiso  yethu khonapho-ke izakuma ukusebenzisa lolo lwazi oluqondane lawe, ngaphandle nxa silobufakazi obuqinileyo obusekela ukuqhubeka kwethu silusebenzisa, kumbe nxa ukulusebenzisa kwethu kuncedisa ukuthola, ukuhambisa, loba ukumela ubufakazi obubekwe phambi komthetho (§ 21 GDPR).</p>
        <p>Ngendlela engaphazamisi izisombululo ezingokokuphatwa kwemisebenzi kumbe ukuqhutshwa komthetho ngemfanelo, ulelungelo lokufaka isikhalazo kubaphathi nxa ukholwa ukuthi ukusetshenziswa kolwazi oluqondane lawe kwephula izimiso zoMthetho weGDPR (§ 77 GDPR). Ungasebenzisa ilungelo lakho leli ngokuya kubaphathi abaselizweni lapho ohlala khona, abaselizweni lapho osebenza khona, kumbe abasendaweni lapho okwephulelwe khona izimiso zomthetho. Nxa useNorth Rhine–Westphalia, umuntu olandelayo nguye oziphetheyo:</p>
        <p>The data protection officer for North Rhine–Westphalia (LDI)<br />
            Kavalleriestrasse 2 - 4<br />
            40213 Düsseldorf Germany</p>

        <h2>8. Amakhukhi Lokunaba Kofinyelelo</h2>
        <p>Amakhukhi yimininingwane yolwazi oludluliselwa phambili luvela kuseva/ isidikidiki yobulembu bethu kumbe kumaseva obulembu bezinye izinhlanganiso esisebenzelana lazo lusiya kumakhasi okungena ebulenjini akukhompiyutha yomsebenzisi, lapho azagcinwa khona ukuze asetshenziswe esikhathini esizayo. Amakhukhi angathatha isimo samafayili amancane kumbe eminye imihlobo yokugcina ulwazi. Malunga lalesi Saziso esiphathelane leMfihlo, uzabikelwa ngamakhukhi wonke asetshenziswa ukulinganisa ukudepha kofinyelelo engathathi lwazi oluqondane lawe siqu sakho. Nxa ungafuni ukuthi kube lamakhukhi agcinwa kukhompiyutha yakho, uyacelwa ukuthi ukhethe lapho okutsho lokho kumasettings ekhasi ongena ngalo ebulenjini. Amakhukhi agciniweyo angacitshwa wonke kumasettings ekhasi ongena ngalo ebulenjini. Ukumisa amakhukhi kungabangela ukuthi iwebhusayithi ingasebenzi ngokupheleleyo kukhompiyutha yakho.</p>
        <p><a href="https://support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies" target="_blank">Internet Explorer:
            support.microsoft.com/en-gb/help/17442/windows-internet-explorer-delete-manage-cookies</a></p>
    
        <p><a href="https://support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences" target="_blank">Firefox:
            support.mozilla.org/en-US/kb/enable-and-disable-cookies-website-preferences</a></p>
    
        <p><a href="https://support.google.com/chrome/answer/95647?hl=en-GB" target="_blank">Google Chrome:
            support.google.com/chrome/answer/95647?hl=en-GB</a></p>
    
        <p><a href="https://support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac" target="_blank">Safari:
            support.apple.com/en-gb/guide/safari/manage-cookies-and-website-data-sfri11471/mac</a></p>
    
        <p><a href="http://help.opera.com/Linux/12.10/en/cookies.html" target="_blank">Opera:
            help.opera.com/Linux/12.10/en/cookies.html</a></p>
        <p>Ulakho njalo ukumisa amakhukhi lawa alinganisa ukudepha kofinyelelo lancedisa ekukhankaseleni izinto ezithengiswayo ngokusebenzisa ikhasi lenethiwekhi elokumisa izikhangiso, iNetwork Advertising Initiative’s deactivation page (<a href="http://optout.networkadvertising.org/" target="_blank">http://optout.networkadvertising.org/</a> lakukhasi lewebhusayithi yeleMelika (<a href="http://www.aboutads.info/choices" target="_blank">http://www.aboutads.info/choices</a>) noma eleYurophu (<a href="http://www.youronlinechoices.com/uk/your-ad-choices/" target="_blank">http://www.youronlinechoices.com/uk/your-ad-choices/</a> Siphinda njalo sisebenzise isizinda sokuholela imvumo esilungiswe ngabe-Usercentrics GmbH. Le yinsiza yokuhola ngayo imvumo enika uluhlu lwezizatho zokuqoqwa kolwazi lokusetshenziswa kwalo. Ulwazi oluqoqiweyo kalusoze lwagcinwa kumbe ukusetshenziselwa esinye isizatho esingekho kulolu luhlu: Ukulandela imilandu engokomthetho, ukugcinwa kwemvumo. Imininingwane ephathelane lemvumo (imvumo enikiweyo kanye lemvumo elanduliweyo) izagcinwa okweminyaka emithathu. Ngemva kwalesi sikhathi, imininingwane yonke iyacitshwa kumbe idluliselwe phambili emuntwini ofaneleyo ngemva kokuthola isicelo sokuthumela ulwazi. Ungahola imvumo zakho ngokuhlaba ikinobho <a href="https://www.welthungerhilfe.org/privacy/#c24231" target="_blank">ephezu kwekhasi leli</a>.</p>

        <h2>9. Isaziso Kubazali Labanakekeli Babantwana</h2>
        <p>Kasivumeli abantwana abangaphansi kweminyaka elitshumi lasitshiyangalombili (18) yokuzalwa ukuthi babhalise ku-AgriShare. Nxa ungaphansi kweminyaka engu18 yokuzalwa, kasilufuni ulwazi oluqondane lawe, njalo sicela ungasiniki lona. Nxa ungumzali kumbe umnakekeli njalo ukholwa ukuthi umntanakho ongaphansi kweminyaka engu18 yokuzalwa usinike ulwazi oluqondane laye, sicela usithinte ukuze sicitshe lolo lwazi oluqondane lomntanakho.</p>

        <h2>10.	Nxa singaguqula lesi Saziso esiphathelane leMfiihlo kumbe ezinye izaziso eziphathelane lemfihlo ke?</h2>
        <p>Kungenzeka kube lesidingo sokuthi sitshintshe isaziso lesi kanye lezinye izaziso. Zonke inguquko zizabhalwa enkundleni zebulenjini. Ukuqhubeka kwakho usebenzisa isizinda kumbe izinsiza zethu ngemva kosuku okuqale ngalo ukusebenza lezo nguquko kutsho ukuthi uyavumelana lazo. Ukuze ukuhlola kwakho lezi nguquko kube lula, sizabhala ilanga lezo nguquko ezizaqala ukusebenza ngalo phezu kwekhasi</p>

        <h2>11.	Imibuzo yakho noma Okunye Ongakutsho</h2>
        <p>Ukhululekile ukuthinta Omkhulu wethu wezokuvikelwa kolwazi nxa ulemibuzo, imibono loba izikhalazo ofisa ukuzinikeza maqondana leSaziso sethu esiphathelane leMfihlo. Ungathinta Omkhulu wethu wezokuvikelwa kolwazi ngencwadi ka-email kukheli ethi <a href="mailto:datenschutz@welthungerhilfe.de" class="s6" target="_blank">datenschutz@welthungerhilfe.de</a> kumbe ngokuthumeza incwadi ngabeposo kukheli yethu, ubhale phezulu ukuthi &quot;KuMkhulu wezokuvikelwa kolwazi&quot; (kumbe &quot;To the data protection officer&quot;). Nxa ufisa ukwazi ngolwazi oluqondane lawe, ukuluguqula noma ukulucitsha, sicela usithinte.</p>
    </div>

    <p><a class="button" onclick="chooseCookies()">Update your cookie preferences</a></p>

</asp:Content>
