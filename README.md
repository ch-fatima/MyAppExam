Input Validation
هر دیتایی که در ورودی هر سرویس قرار می گیرد غیرقابل اعتماد است.
•	از WhiteList به جای BlackList استفاده شود.
Check List : Code Review
	آیا برای هر ورودی، نوع داده (string/int/bool)، Length، Format و Range استفاده شده است؟
	آیا از Whitelist validation استفاده شده نه Blacklist؟
	آیا در SQL Queryها از Parameterized Query/ORM برای ایجاد کوئری استفاده شده؟
	آیا در فرمان سیستمی مانند exec()، ورودی با shlex.quote()  یا روش معادل امن شده است؟
Authentication&Authorization
کاربران تنها با توکن معتبر و دسترسی مشخصی مجاز به استفاده از سرویس های مربوط باشند.
Check List : Code Review
	آیا همه endpointها به جز صفحات عمومی مانند  (login, register, recovery) نیاز به احراز هویت دارند؟
	آیا پس از لاگین، توکن (JWT) در سمت کلاینت امن ذخیره می‌شود؟ HttpOnly, Secure, SameSite cookies برای جلوگیری از XSS
	آیا توکن‌ها زمان انقضا کوتاه مثلاً 10 دقیقه برای access token دارند و از Refresh Token استفاده می‌شود؟ 
	آیا رمز عبور در دیتابیس به صورت hash شده مثل bcrypt, argon2) ذخیره می‌شود (نه plain text و نه hash ضعیف مثل MD5؟
	آیا دسترسی‌ها بر اساس Role-based (RBAC) یا Attribute-based (ABAC) پیاده شده و در هر درخواست چک می‌شود؟ 
Logging
 لاگ‌ها باید به رفع مشکل کمک کنند، اما هرگز نباید شامل اطلاعات حساس (مثل رمز، توکن، شماره کارت بانکی) شوند.
Check List : Code Review
	آیا لاگ شامل password, token, Authorization header, credit card, SSN می‌شود؟ 
	آیا خطاها تمام جزئیات Stack Trace را به کاربر نهایی نمایش نمی‌دهند ؟
	آیا Logg Level به درستی رعایت شده است؟
	آیا در لاگ‌های خروجی، UID یا Session ID به جای داده مستقیم کاربر ذخیره می‌شود؟
Secrets & Credentials 
	آیا هیچ کلید API، رمز عبور، توکن (JWT, OAuth, SSH) به صورت متن آشکار در کد نوشته شده؟
	آیا فایل‌های .env, .secrets, *.pem, *.key در .gitignore قرار دارند؟
	آیا از ابزار اسکن خودکار (مثل gitleaks, trufflehog, detect-secrets) در CI/CD استفاده می‌شود؟
	آیا دسترسی به secrets از طریقAWS Secrets Manager، Azure Key Vault، GitHub Secrets) انجام شده؟
	آیا هیچ کامنتی حاوی TODO: hardcoded secret یا نمونه secret در کد وجود ندارد؟
Security Culture & Training
	آیا تیم در ۳۰ روز گذشته یک جلسه شبیه‌سازی «یافتن آسیب امنیتی در کد» برگزار کرده است؟
	آیا یک مرجع سریع همیشه در کانال تیم یا Confluence در دسترس است؟
	آیا ابزارهای لینتر امنیتی برای JS در CI/CD فعال هستند؟
	آیا فرد جدید تیم در روز اول آموزش "Secure Coding" و نحوه استفاده از Vault را دیده است؟
	آیا مستند است که در صورت کشف اشتباه امنیتی مثل commit secret سرزنش فردی وجود ندارد، بلکه فرآیند اصلاح می‌شود؟
