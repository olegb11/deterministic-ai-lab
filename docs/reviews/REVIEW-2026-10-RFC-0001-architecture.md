# Архітектурна рецензія RFC-0001 (Intent Compilation Pipeline), v0.4

| | |
|---|---|
| **Рецензент** | Микола Федчик (Mykola Fedchyk) |
| **Автор RFC** | Олег Бурлака (Oleg Burlaka) |
| **Предмет** | `docs/rfcs/RFC-0001-ICP-Public-Draft.md` і його реалізація в гілці `refactor/icp-audit-remediation` |
| **Версія, що рецензується** | коміт `e56cbe7` |
| **Метод** | статичне читання RFC, специфікацій, тестів, скриптів і CI; тести та Stryker **не запускалися**; підготовлено із залученням Claude (Anthropic) |
| **Питання автора** | чи життєздатна модель з погляду інженерії та формалізації знань, чи є концептуальні хиби |
| **Попередня рецензія** | [Review_M_Fedchyk.md](../../Review_M_Fedchyk.md); її зауваження щодо гігієни кодової бази (маркери `[cite]`, заглушки `Class1`, округлення, CI, `.sh`) у цій версії закрито і тут не повторюються |

Посилання на монографію: М. Федчик, *Архітектура доказових експертних систем* ([репозиторій](https://github.com/nick-fedchik/articles/tree/main/ExpertSystem)). Скорочення: **Гл. N §M** означає главу N, розділ M. Посилання на книгу пояснюють підстави зауважень і не є вимогою приймати її термінологію.

Рівні: **Блокер** означає, що твердження RFC не виконується реалізацією. **Суттєве** означає, що твердження RFC не обґрунтоване. **Дрібне** означає, що виправлення займе мінуту.

---

## 1. Вердикт

Модель життєздатна, якщо звузити її заяви. Ядро таке: ШІ генерує кандидата, а детерміновані шлюзи допускають або відхиляють його. Це правильний розподіл ролей, і він збігається з принципом книги «статистичний генератор проти символьного верифікатора» ([Гл. 29 §3](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch29-neuro-symbolic-architecture.md), [Гл. 39 §7.2](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch39-active-compliance-auditor-and-popperian-testing.md)).

Проблеми в трьох місцях:

1. RFC обіцяє **доказ** там, де є лише **свідчення** (Розділ 2).
2. Два ключові механізми, онтологічне заземлення й відкат, **реалізовані навпаки** відносно RFC (Розділ 3).
3. Між Layer 1 і Layer 2 немає ланки, яка б гарантувала відсутність Intent Leakage, тобто саме того, заради чого задумано пайплайн (пункт C3).

---

## 2. Концептуальні зауваження

### C1. «Deterministic» стосується шлюзів, а не генерації — Суттєве

Цілі «Deterministic implementation behavior» і «Reproducible AI execution» ([RFC, Design Goals](../rfcs/RFC-0001-ICP-Public-Draft.md#L41-L50)) недосяжні. LLM не відтворюється повністю навіть при `temperature=0`: змінюються версії моделі, квантування, батчинг. Детермінованим може бути **допуск** результату.

**Пропозиція.** Переформулювати: недетермінована генерація, детермінований допуск. Для відтворюваності зафіксувати маніфест запуску (модель і хеш ваг, шаблон запиту, параметри декодування, версії специфікацій), який дає змогу вимірювати частку розбіжностей між прогонами. Див. [Гл. 23 §10.1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md) (маніфест відповіді та метрика розбіжностей), [Гл. 29 §1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch29-neuro-symbolic-architecture.md) (епістемічний розрив між правдоподібністю й істинністю).

### C2. Аксіоми 4 і 5 та колонка «Proven Guarantee» — надмірні твердження — Суттєве

- Аксіома 4 «Passing tests prove conformance to specification»: тести показують відсутність відмов на перевірених прикладах, а не відповідність специфікації.
- Аксіома 5 «Killed mutants provide evidence of intent preservation»: вбитий мутант показує, що тести **чутливі до змін коду**. Чи кодують тести намір, він не показує.
- Мутація не виявляє **пропущених вимог**: те, чого немає в коді, не мутується.
- Колонка «Proven Guarantee» в Evidence Model ([RFC](../rfcs/RFC-0001-ICP-Public-Draft.md#L146)) має називатися «Evidence for».

**Пропозиція.** Замінити «prove» на «provide evidence» і додати до RFC явне розмежування верифікації (код проти специфікації) та валідації (специфікація проти наміру). Див. [Гл. 23 §1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md) (верифікація проти валідації), [Гл. 23 §7](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md) («високий бал не доводить відсутності всіх дефектів»), [Гл. 2 §12](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch02-epistemology-of-machine-knowledge.md) (формальний критерій права на твердження).

### C3. Немає трасування Layer 1 → Layer 2 — Суттєве

Ніщо не перевіряє, що кожен сценарій та кожен інваріант мають перевіряючий тест. Intent Leakage виникає саме між специфікацією і тестами, а мутаційний шлюз покриває лише зв'язок «тести → код».

**Пропозиція.** Додати Gate між Layer 1 і Layer 2: кожен `@invariant:X` і кожен сценарій мають ≥ 1 тест, який його перевіряє, і зв'язок перевіряється машиною, а не декларується. Тег у тесті є твердженням розробника; дієвим зв'язок стає, коли є вимірюваний слід виконання. Див. [Гл. 9 §3.3](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch09-engineering-knowledge-graph-traceability.md) (ребра `verifies` / `exercises` і вузол свідчення), [Гл. 9 §4.1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch09-engineering-knowledge-graph-traceability.md) (метрики покриття вимог), [Гл. 9 §5.2](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch09-engineering-knowledge-graph-traceability.md) (шлюз у CI).

### C4. Метафора «компіляції» вводить в оману — Дрібне

Компілятор зберігає семантику за побудовою. Тут застосовується «згенеруй і перевір». Твердження «Each stage is a more formal representation of the previous» ([RFC, Core Principle](../rfcs/RFC-0001-ICP-Public-Draft.md#L52-L58)) неточне: тести не формальніші за Gherkin, вони виконують іншу функцію. У книзі «компіляція» завжди завершується детермінованою перевіркою хостом, а не довірою до самої трансформації ([Гл. 13 §3.4](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch13-language-variability-vs-determinism.md)).

**Пропозиція.** Описати пайплайн як *verification-guided generation* або залишити «compilation» як метафору з явним застереженням.

### C5. Layer 0 не містить інваріантів — Суттєве

RFC визначає Layer 0 як технологічно незалежні онтологію та інваріанти. Фактично M1, M2, P1 існують лише в XML-коментарях C# ([Money.cs](../../src/Domain/DeterministicAiLab.Domain/ValueObjects/Money.cs#L8-L9), [PromoCodeText.cs](../../src/Domain/DeterministicAiLab.Domain/ValueObjects/PromoCodeText.cs#L8)) і як теги `@invariant:` у feature-файлі. Файл `ontology.json` є глосарієм без аксіом, кардинальностей чи обмежень.

**Пропозиція.** Винести інваріанти в Layer 0 як окремі пронумеровані артефакти (id, формулювання, граничні значення, посилання на сценарії). Див. [Гл. 14 §3](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch14-requirements-detection-and-formalization.md) (від нормативного тексту до формальних інваріантів), [Гл. 23 §6](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md) (інваріанти, граничні стани, контрприклади).

### C6. Межу «AI Excluded» ніщо не забезпечує — Суттєве

Таблиця повноважень ([RFC, Human vs AI Authority Model](../rfcs/RFC-0001-ICP-Public-Draft.md#L153-L164)) виключає ШІ з Layer 0–2, але механізму примусу немає. ШІ, що правитиме тести, зеленитиме «власні» тести. Це та сама проблема самооцінювання, що й LLM-as-a-judge ([Гл. 38 §1.1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch38-curing-machine-hallucinations-and-knowledge-deficits.md): відмови спільного режиму).

**Пропозиція.** Додати **Gate 0**: ШІ-патч змінює лише шляхи Layer 3 (`src/`). Реалізується через CODEOWNERS, перевірку diff у CI або ізольовану пісочницю. Для частини тестів розглянути запечатаний набір, недоступний генератору ([Гл. 36 §5.6](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch36-knowledge-testing-pyramid-and-variational-calibration.md): held-out sealed gate; [Гл. 39 §7.1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch39-active-compliance-auditor-and-popperian-testing.md): змагальний поділ ролей Red Teamer / Formal Implementer).

### C7. Layer 1.5 і фаза Refactor не мають шлюзу — Дрібне

У Evidence Model немає рядка для Technical Constraints, а фаза Refactor відсутня в Definition of Done. Або додати відповідні перевірки, або прибрати їх з моделі.

---

## 3. Розбіжності між RFC і реалізацією

### I1. Перевірка онтології працює у зворотному напрямі — Блокер

Semantic Leakage ([RFC, Terminology](../rfcs/RFC-0001-ICP-Public-Draft.md#L20-L23)) це термін у специфікації, якого немає в онтології. Тест [OntologyValidationTests.cs](../../tests/DeterministicAiLab.Application.Tests/Architecture/OntologyValidationTests.cs) перевіряє навпаки: що кожен термін онтології **зустрічається** у feature-файлах ([L94](../../tests/DeterministicAiLab.Application.Tests/Architecture/OntologyValidationTests.cs#L94)).

Додаткові дефекти:

- збіг через `Contains` без меж слів, тож «Cart» знаходиться у будь-якому слові;
- якщо каталогу `specs` або файлів `.feature` немає, тест просто повертається й **проходить** ([L53-L61](../../tests/DeterministicAiLab.Application.Tests/Architecture/OntologyValidationTests.cs#L53-L61));
- змінні `allowedTermsAndPhrases` і `missingTerms` обчислюються, але не впливають на результат (мертвий код, [L27](../../tests/DeterministicAiLab.Application.Tests/Architecture/OntologyValidationTests.cs#L27), [L67](../../tests/DeterministicAiLab.Application.Tests/Architecture/OntologyValidationTests.cs#L67));
- зв'язки `relationships` в `ontology.json` не перевіряються взагалі.

Приклад наслідку описано в I5.

**Пропозиція.** Витягувати терміни зі сценаріїв (токени кроків Gherkin, іменовані сутності) і вимагати, щоб кожен належав замкненому переліку онтології, за зразком верифікатора хоста: намір належить замкненому переліку, типи сумісні, джерело дозволене ([Гл. 13 §3.4](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch13-language-variability-vs-determinism.md)). Порожній набір специфікацій має **падати**, а не проходити. Це приклад вакуумної істинності ([Гл. 36 §3.3](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch36-knowledge-testing-pyramid-and-variational-calibration.md)).

### I2. Відкат суперечить RFC і руйнує роботу людини — Блокер

RFC: «immediate, deterministic rollback to the last known green state (e.g., via `git reset --hard`)» ([RFC L123](../rfcs/RFC-0001-ICP-Public-Draft.md#L123)). Скрипт [run-tdd-cycle.sh:20](../../run-tdd-cycle.sh#L20) виконує `git restore src/ tests/`. Наслідки:

- відновлюється **`tests/`**, тобто незакомічені червоні тести людини (Layer 2) знищуються; а Крок 1 у фазі Red падає за означенням;
- нові файли, створені ШІ, не видаляються, бо `git restore` не торкається untracked;
- збій Stryker ([L43](../../run-tdd-cycle.sh#L43)) завершується `exit 1` без жодного відкату, хоча RFC вимагає відкату при будь-якому провалі.

**Пропозиція.** Відкат стосується лише Layer 3: `git restore --source=HEAD -- src/` плюс видалення untracked у `src/` (`git clean -fd -- src/`), і ніколи не `tests/`. Червоні тести людини зберігаються окремим комітом до запуску ШІ, тоді «last known green» чітко визначений. Див. [Гл. 23 §13](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md) (керовані зміни та версіонування).

### I3. Provenance самозаявлений і не є git trailers — Суттєве

- Кілька `-m` створюють окремі абзаци ([run-tdd-cycle.sh:61-64](../../run-tdd-cycle.sh#L61-L64)), а `git interpret-trailers` читає лише останній. Реально розпізнається тільки `Pipeline-Verdict`. Перевірити можна так: `git log -1 --format='%(trailers)'`.
- `Mutation-Score: 100.0%` ([L50](../../run-tdd-cycle.sh#L50)) і `DETERMINISTIC_COMPILATION_PASSED` є захардкодженими рядками, а не значеннями зі звіту Stryker.
- Хеш `Intent-Spec-Hash` береться з **одного** `.feature`, тоді як RFC обіцяє «feature specification **and ontology**»; хеша тестів немає; «Verification Results» з RFC у trailers відсутній.
- Підпису немає: будь-хто може вручну написати такі рядки в коміт. Це твердження, а не доказ.

**Пропозиція.** Один суцільний блок trailers (`-m "$MSG" -m "$(printf 'A: x\nB: y')"` або `--trailer`), значення зі звітів, підписані коміти (`git commit -S`), звіт Stryker як артефакт CI з прив'язкою до SHA. Див. [Гл. 9 §5.1](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch09-engineering-knowledge-graph-traceability.md) (незмінний лог із контекстом походження й підписом інструмента), [Гл. 27 §5](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch27-safety-case-gsn-synthesis.md) (криптографічна цілісність аргументу).

### I4. Gate 2 не виконується в CI — Суттєве

[ci.yml](../../.github/workflows/ci.yml) запускає лише `run-tests.sh`. Stryker у CI відсутній, тож «100% kill» з Definition of Done ([RFC L179](../rfcs/RFC-0001-ICP-Public-Draft.md#L179)) ніхто не перевіряє на сервері, і гарантія тримається лише на локальній дисципліні. Див. [Гл. 9 §5.2](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch09-engineering-knowledge-graph-traceability.md).

### I5. Сценарій «expired» не перевіряє expired — Суттєве

Сценарій називається «expired **or** invalid format promo code» ([shopping-cart-policy.feature:15](../../docs/specs/shopping-cart-policy.feature#L15)), але єдиний крок перевіряє лише невалідний формат (`BAD1`). Термін `Expired` з онтології «знаходиться» тестом I1 завдяки слову в заголовку. Це живий приклад Semantic Leakage, який RFC має виявляти, а механізм не виявляє. Антецедент «код прострочений» жодного разу не активується ([Гл. 36 §3.3](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch36-knowledge-testing-pyramid-and-variational-calibration.md)); межу строку дії слід перевіряти граничними значеннями ([Гл. 36 §3.4](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch36-knowledge-testing-pyramid-and-variational-calibration.md)).

### I6. Кроки BDD не залежать від специфікації — Суттєве

- Крок «applies the promo code "SAVE10"» ([StepDefinitions:30](../../tests/DeterministicAiLab.Domain.BDD.Tests/StepDefinitions/ShoppingCartStepDefinitions.cs#L30)) створює знижку із захардкодженим `10.00m`, не читаючи її з коду. Тест зеленіє за збігом числа з назвою.
- «A domain validation exception should be thrown» перевіряє лише `NotBeNull` ([L77](../../tests/DeterministicAiLab.Domain.BDD.Tests/StepDefinitions/ShoppingCartStepDefinitions.cs#L77)): підійде будь-який виняток, зокрема `NullReferenceException`.
- Суми парсяться як `float` і кастуються в `decimal` ([L16](../../tests/DeterministicAiLab.Domain.BDD.Tests/StepDefinitions/ShoppingCartStepDefinitions.cs#L16)), що суперечить меті інваріанта M2 щодо точних грошових розрахунків; краще `{decimal}`.

---

## 4. Зауваження до RFC-0002 (Mutation Verification) і RFC-0003 (Equivalent Mutant Admission)

Спершу закрийте пункти C2, C3, C6 та I1–I4, потім розвивайте RFC-0002/0003. Вимога до змісту RFC-0003:

1. **Нерозв'язність.** Визначення еквівалентності в загальному випадку алгоритмічно нерозв'язне. «100% kill» без процедури розгляду вцілілих мутантів невиконувана, а автоматичне списання вцілілих на «еквівалентність» перетворює шлюз на фікцію. Розв'язок: або формальний доказ (наприклад, SMT), або новий тест ([Гл. 23 §7](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md), блок про семантичну пастку еквівалентних мутантів).
2. **Формула.** MS = killed / (total − equivalent). Якщо придатних нееквівалентних мутантів немає, бал **невизначений**, а не 100%. Невалідні кандидати, помилки запуску й непідтверджена еквівалентність мають окремі статуси.
3. **Допуск є людським рішенням** (це випливає з вашої таблиці повноважень): ШІ не може сам допустити вцілілого мутанта. Запис у реєстрі прив'язується до **хеша мутанта і хеша вихідного коду**, тож зміна коду скидає допуск. Сам реєстр має бути під Gate 0.
4. **Метрика допусків.** Кількість і частка допущених мутантів звітуються окремо, щоб реєстр не став лазівкою.
5. **Критичні інваріанти.** Вцілілий мутант у правилі безпеки блокує випуск незалежно від загального балу ([Гл. 23 §7](https://github.com/nick-fedchik/articles/blob/main/ExpertSystem/ch23-knowledge-base-verification.md)).
6. **Рівні допуску.** У книзі поріг залежить від критичності (MS ≥ 0,95 для функціональної безпеки при нульовій кількості вцілілих у критичних інваріантах). Варто розглянути, чи потрібен фіксований 100% для кожного проєкту, чи профілі за критичністю.

---

## 5. Мінімальний шлях до сильнішої версії

| № | Дія | Закриває |
|---|---|---|
| 1 | Переписати цілі й аксіоми 4–5 у категоріях «evidence» та додати верифікацію/валідацію | C1, C2 |
| 2 | Виправити напрям перевірки онтології; порожній набір специфікацій має падати | I1, I5 |
| 3 | Додати автоматичну перевірку «інваріант/сценарій → тест» | C3 |
| 4 | Gate 0: ШІ змінює лише `src/` | C6 |
| 5 | Вирівняти `run-tdd-cycle.sh` з RFC: відкат лише Layer 3, справжні trailers зі звітів, підпис | I2, I3 |
| 6 | Stryker у CI з артефактом звіту | I4 |
| 7 | Прибрати захардкоджене з step definitions; звузити перевірку винятку | I6 |
| 8 | Далі: RFC-0002 і RFC-0003 за Розділом 4 | |

## 6. Обмеження цієї рецензії

- Код не виконувався; висновки про поведінку скриптів і тестів зроблено за текстом. Перед виправленням варто відтворити пункти I2 і I3 командами, які наведено вище.
- Не оцінювалися безпека ASP.NET-шару, схема БД і продуктивність.
- Посилання на монографію пояснюють підстави, але не замінюють аргументації. Якщо зауваження здається необґрунтованим, його слід оскаржити по суті.
