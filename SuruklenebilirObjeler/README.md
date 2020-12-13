# Sürüklenebilir Objeler
Yaptığım bu basit örnek ile, Rigidbody2d tabanlı bir sürükleme işlemini gerçekleştiriyoruz. En büyük artısı tabii ki çevre ve diğer objelerle etkileşim halinde olunabilmesi. 
Yorum satırlarından daha detaylı bilgiler bulabilirsiniz. Ancak aşağıda bahsettiğim kısımlara dikkat etmelisiniz.

## Yapılması Gerekenler
1. "Edit>Project Settings>Physics2D" ayarları altında "Queries Start in Collider" seçeneğinin deaktif edilmesi gerekiyor.
	1. Bunun nedeni obje içinde başlattığımız linecast işleminin objeye çarpıyor olması. Biraz kolaya kaçtığım için bu şekilde bir ayar yaptım. Eğer bu özelliğe ihtiyacınız varsa, kodlama kısmında "LineCast" yerine "LineCastAll" kullanarak, engelleri algılama kısmında yol boyunca bütün objeleri alabilirsiniz. Sürüklenen objenin kendisi olmayan ilk obje, karşınızdaki aradaki ilk engel olacaktır.
2. 	Sürükleneblir haldeki objelerinizin belirli bir tag içerisinde olması gerekiyor. Genel kullanıma dair aslında bu iyi bir pratik değil. Tag yapısını oyununuzda farklı bir sistemde kullanmak isteyebilirsiniz, ki çok daha mantıklı olur burada. Bunun farkındayım, ancak bunun da bir problem durumu olarak sizin çözmenizi istiyorum. İpucu olarak vereceğim tek şey de "LayerMask".
