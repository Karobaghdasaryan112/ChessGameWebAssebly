# ChessGameWebAssembly - Deep Project Logic and Product Ideas
# ChessGameWebAssembly - Նախագծի խորը տրամաբանություն և գաղափարներ

## 1) Product Vision
## 1) Արտադրանքի տեսլական

This project is designed as a real-time chess platform that combines competitive multiplayer gameplay and training mode against AI in one unified architecture.  
The core idea is to make the chess experience responsive, stateful, and event-driven, so that each move is both a game action and a synchronized system event.

Այս նախագիծը նախագծված է որպես իրական ժամանակի շախմատի հարթակ, որը միավորում է մրցակցային բազմախաղացող ռեժիմը և AI-ի դեմ մարզման ռեժիմը մեկ միասնական ճարտարապետության մեջ։  
Հիմնական գաղափարն այն է, որ շախմատի փորձառությունը լինի արձագանքող, վիճակային և իրադարձությունակենտրոն, որպեսզի յուրաքանչյուր քայլ լինի ոչ միայն խաղային գործողություն, այլև համաժամեցված համակարգային իրադարձություն։

---

## 2) Architectural Logic
## 2) Ճարտարապետական տրամաբանություն

### 2.1 Layered structure with clear responsibilities
### 2.1 Շերտավորված կառուցվածք՝ հստակ պատասխանատվություններով

The solution separates concerns into multiple domains:
- `BlazorServerSideClient`: UI interaction, authentication context, JS interop bridge, and user-driven game actions.
- `ChessGame.Infrastructure.Infrastructure`: Hub and transport-facing services for real-time communication.
- `ChessGame.Core.Services`: command/query orchestration (MediatR), board logic, move logic, king safety logic, and AI decision-making.
- `SharedResources`: board models, figures, DTO contracts, enums, and static in-memory states.

Լուծումը տարանջատում է պատասխանատվությունները մի քանի շերտերի մեջ.
- `BlazorServerSideClient`: UI փոխազդեցություն, նույնականացման կոնտեքստ, JS interop շերտ, և օգտատիրոջ գործողություններով պայմանավորված խաղային հարցումներ։
- `ChessGame.Infrastructure.Infrastructure`: Hub և տրանսպորտային (real-time) հաղորդակցման ծառայություններ։
- `ChessGame.Core.Services`: հրաման/հարցում օրկեստրացիա (MediatR), խաղատախտակի տրամաբանություն, քայլերի տրամաբանություն, թագավորի անվտանգության տրամաբանություն և AI որոշումների ընդունում։
- `SharedResources`: խաղատախտակի մոդելներ, խաղաքարեր, DTO պայմանագրեր, enum-ներ և հիշողության մեջ պահվող վիճակներ։

### 2.2 Real-time first communication model
### 2.2 Իրական ժամանակի առաջնային հաղորդակցման մոդել

`GameHub` acts as the single real-time entry point for game lifecycle calls:
- invitation flow (`SendInviteAsync`, `AcceptInviteAsync`)
- connection flow (`AddConnectionAsync`, `RemoveConnectionAsync`)
- gameplay flow (`SendClickAsync`, `SendMoveAsync`, training game request)

`GameHub`-ը հանդիսանում է իրական ժամանակի միասնական մուտքի կետը խաղային կյանքի ցիկլի համար.
- հրավերների հոսք (`SendInviteAsync`, `AcceptInviteAsync`)
- կապի հոսք (`AddConnectionAsync`, `RemoveConnectionAsync`)
- gameplay հոսք (`SendClickAsync`, `SendMoveAsync`, մարզման խաղի հարցում)

This aligns with your central logic idea: one consistent orchestration point for all live game states.

Սա համընկնում է ձեր հիմնական տրամաբանական գաղափարի հետ՝ մեկ հետևողական օրկեստրացիոն կետ բոլոր live խաղային վիճակների համար։

---

## 3) Core Game Loop and State Flow
## 3) Հիմնական խաղային ցիկլ և վիճակի հոսք

### 3.1 In-memory game state as active runtime context
### 3.1 Հիշողության մեջ պահվող խաղային վիճակը որպես ակտիվ runtime կոնտեքստ

`ActiveGames` keeps:
- `ConcurrentDictionary<Guid, Board>` for active board states by game ID.
- `ConcurrentDictionary<Guid, UserConnectionDTO>` for online users and connection metadata.

`ActiveGames`-ը պահում է.
- `ConcurrentDictionary<Guid, Board>` ակտիվ տախտակների վիճակները՝ ըստ game ID-ի։
- `ConcurrentDictionary<Guid, UserConnectionDTO>` առցանց օգտատերերի և կապի մետատվյալների համար։

The logic is optimized for low-latency play by avoiding database reads on every move.

Տրամաբանությունը օպտիմիզացված է ցածր ուշացումով խաղի համար՝ խուսափելով յուրաքանչյուր քայլին տվյալների բազայից կարդալուց։

### 3.2 Move handling lifecycle
### 3.2 Քայլի մշակման ցիկլ

The move pipeline is cleanly layered:
1. Client sends move through hub service.
2. `GameService.SendMoveAsync` validates request and determines event readiness.
3. `MoveLogicCommandHandler` executes game-level move orchestration.
4. `SubmitMoveCommandHandler` applies move + verifies king safety.
5. Position is serialized to FEN and persisted.
6. Turn is switched and board state is broadcast to both clients.

Քայլի մշակման շղթան շերտավորված է.
1. Հաճախորդը hub-ի միջոցով ուղարկում է քայլը։
2. `GameService.SendMoveAsync`-ը վավերացնում է հարցումը և որոշում է event readiness-ը։
3. `MoveLogicCommandHandler`-ը կատարում է խաղի մակարդակի օրկեստրացիան։
4. `SubmitMoveCommandHandler`-ը կիրառում է քայլը և ստուգում թագավորի անվտանգությունը։
5. Դիրքը սերիալիզացվում է FEN-ի և պահպանվում։
6. Քայլը փոխանցվում է հաջորդ կողմին, և տախտակի վիճակը ուղարկվում է երկու հաճախորդներին։

This reflects a strong engineering idea: keep UI dumb, keep domain logic authoritative.

Սա արտացոլում է ուժեղ ինժեներական գաղափար՝ UI-ն պահել թեթև, իսկ դոմեն տրամաբանությունը պահել որպես հիմնական ճշմարտության աղբյուր։

---

## 4) Rule Enforcement and Chess Correctness
## 4) Կանոնների Կիրառում և շախմատի ճշտություն

Your logic already includes several correctness safeguards:
- Illegal move blocking when no figure exists on source.
- Event color checks (`Move`, `Cut`, `Castle`) before move execution.
- Temporary move simulation + revert if move leaves own king in check.
- Castling rook relocation during castle events.
- Explicit checked-king targeting for client-side highlight.

Ձեր տրամաբանությունն արդեն ներառում է մի շարք կարևոր պաշտպանիչ մեխանիզմներ.
- Անօրինական քայլի արգելափակում, եթե սկզբնական դաշտում խաղաքար չկա։
- `Move` / `Cut` / `Castle` event գույների ստուգում մինչև քայլի կատարումը։
- Ժամանակավոր քայլի սիմուլյացիա և հետադարձ, եթե քայլից հետո սեփական թագավորը գտնվում է շախի տակ։
- Ռոխադայի ժամանակ նավակի ճիշտ տեղափոխում։
- Շախ ստացած թագավորի դիրքի հստակ փոխանցում հաճախորդին՝ վիզուալ նշման համար։

This is a practical logic model: validate intent, apply simulation, commit only legal outcome.

Սա պրագմատիկ տրամաբանական մոդել է՝ վավերացնել մտադրությունը, կիրառել սիմուլյացիա, հաստատել միայն օրինական արդյունքը։

---

## 5) AI Logic and Decision Design
## 5) AI տրամաբանություն և որոշումների ձևավորում

The training mode AI follows a meaningful strategy pipeline:
- Alpha-beta search (`GetOptimizedMoveQueryHandler`).
- Depth controlled by `TrainingDifficulty` through `HelperConstants.MAX_DEPTH`.
- Board evaluation using:
  - material score (`FigureScores`)
  - positional score (piece-square tables)
  - king safety and mate-state scoring.

Մարզման ռեժիմի AI-ն հետևում է կառուցվածքային ռազմավարական հոսքի.
- Alpha-beta որոնում (`GetOptimizedMoveQueryHandler`)։
- Որոնման խորությունը վերահսկվում է `TrainingDifficulty`-ով (`HelperConstants.MAX_DEPTH`)։
- Տախտակի գնահատում հետևյալ բաղադրիչներով.
  - նյութական արժեք (`FigureScores`)
  - դիրքային արժեք (piece-square table-ներ)
  - թագավորի անվտանգություն և մատ վիճակի գնահատում։

This is a solid idea foundation: combine tactical legality with strategic scoring.

Սա ամուր գաղափարական հիմք է՝ տակտիկական օրինականության և ռազմավարական գնահատման համադրություն։

### AI concept direction you are implementing well
### AI գաղափարական ուղղություն, որը լավ եք իրականացնում

Your current AI is not random and not purely material-based; it already has positional and mate pressure awareness.  
That means your project is positioned as "training quality chess", not just "playable chess".

Ձեր ներկայիս AI-ն ոչ պատահական է և ոչ էլ միայն նյութական հավասարակշռության վրա հիմնված. այն արդեն ունի դիրքային և մատի սպառնալիքի զգայունություն։  
Սա նշանակում է, որ նախագիծը դիրքավորվում է որպես «մարզողական որակի շախմատ», ոչ թե պարզապես «խաղալու հնարավորությամբ շախմատ»։

---

## 6) Multiplayer Session Logic
## 6) Բազմախաղացող սեսիայի տրամաբանություն

Your multiplayer design follows a practical social game lifecycle:
1. User comes online and registers connection metadata.
2. User invites another player.
3. Invitation acceptance creates game + board + group membership.
4. Both users receive synchronized board updates.
5. Disconnect handling notifies opponent and resolves session consistency.

Ձեր բազմախաղացող դիզայնը հետևում է սոցիալական խաղի գործնական կյանքի ցիկլին.
1. Օգտատերը առցանց է գալիս և գրանցում կապի տվյալները։
2. Օգտատերը հրավիրում է մեկ այլ խաղացողի։
3. Հրավերի ընդունումը ստեղծում է խաղ + տախտակ + group membership։
4. Երկու օգտատերն էլ ստանում են համաժամեցված տախտակի թարմացումներ։
5. Անջատման դեպքում համակարգը ծանուցում է մրցակցին և պահպանում սեսիայի ամբողջականությունը։

This supports your core product idea: "real-time chess as an interaction system", not only a board engine.

Սա աջակցում է ձեր հիմնական արտադրանքային գաղափարին՝ «իրական ժամանակի շախմատ որպես փոխազդեցության համակարգ», ոչ միայն որպես տախտակի շարժիչ։

---

## 7) Your Current Strengths (Logic + Engineering)
## 7) Ձեր ներկայիս ուժեղ կողմերը (տրամաբանություն + ինժեներիա)

- MediatR-based decomposition keeps complex rules understandable and testable.
- Validation-first behavior reduces invalid state transitions early.
- Consistent DTO contracts make client-server communication explicit.
- Event-based board flags simplify front-end rendering logic.
- AI pipeline is already extendable for stronger heuristics.

- MediatR-ով դեկոմպոզիցիան բարդ կանոնները դարձնում է ընկալելի և թեստավորվող։
- Validation-first մոտեցումը վաղ փուլում նվազեցնում է սխալ վիճակների անցումները։
- Հետևողական DTO պայմանագրերը հստակեցնում են client-server հաղորդակցությունը։
- Event-ների վրա հիմնված տախտակի նշումները պարզեցնում են front-end արտապատկերման տրամաբանությունը։
- AI pipeline-ը արդեն ընդլայնելի է ավելի ուժեղ heuristic-ների համար։

---

## 8) Product Ideas You Can Build Next
## 8) Հաջորդ արտադրանքային գաղափարներ, որոնք կարող եք կառուցել

### 8.1 Gameplay and user value
### 8.1 Gameplay և օգտատիրոջ արժեք

- Add PGN export/import for game analysis portability.
- Add move history panel with annotations and blunder detection tags.
- Add rematch workflow with color swap and same lobby continuity.
- Add reconnection grace window (for short network drops).

- Ավելացնել PGN արտահանում/ներմուծում՝ խաղի վերլուծության տեղափոխելիության համար։
- Ավելացնել քայլերի պատմության վահանակ՝ նշումներով և խոշոր սխալների պիտակավորմամբ։
- Ավելացնել ռևանշի հոսք՝ գույների փոխանակմամբ և նույն lobby-ի շարունակականությամբ։
- Ավելացնել reconnect-ի grace window (կարճ ցանցային ընդհատումների համար)։

### 8.2 AI and training depth
### 8.2 AI և մարզման խորություն

- Transposition table (hash-based cache) to avoid recomputing equivalent positions.
- Move ordering (captures/checks first) to improve alpha-beta pruning efficiency.
- Quiescence search on tactical edges to reduce horizon effect.
- Opening book for first moves and endgame tablebase integration for late phase.

- Transposition table (hash cache)՝ համարժեք դիրքերի կրկնվող հաշվարկները նվազեցնելու համար։
- Քայլերի դասակարգում (նախևառաջ խփումներ/շախեր)՝ alpha-beta pruning-ի արդյունավետությունը բարձրացնելու համար։
- Quiescence search տակտիկական դիրքերում՝ horizon effect-ը նվազեցնելու համար։
- Opening book սկզբնական քայլերի համար և endgame tablebase ինտեգրում վերջնախաղի փուլում։

### 8.3 Reliability and operations
### 8.3 Հուսալիություն և շահագործում

- Persist active game snapshots periodically, not only move positions.
- Add structured telemetry for move latency and hub message health.
- Add deterministic replay mode from saved FEN sequence.
- Add integration tests for disconnect and reconnect race scenarios.

- Պարբերաբար պահել ակտիվ խաղի snapshot-ներ, ոչ միայն քայլերի դիրքերը։
- Ավելացնել structured telemetry՝ քայլի ուշացման և hub հաղորդագրությունների առողջության համար։
- Ավելացնել deterministic replay ռեժիմ՝ պահպանված FEN հաջորդականությունից։
- Ավելացնել ինտեգրացիոն թեստեր՝ disconnect/reconnect մրցավազքային սցենարների համար։

---

## 9) Long-Term Technical Direction
## 9) Երկարաժամկետ տեխնիկական ուղղություն

If your goal is to grow into a production-grade chess platform, your current architecture already supports that path:
- You have modular command/query boundaries.
- You have clear real-time interaction contracts.
- You have an AI engine with extensible evaluation logic.

To scale confidently, focus next on:
- stronger deterministic tests around move legality and king safety,
- state recovery strategy for hub disconnections,
- AI performance profiling by depth and branching factor.

Եթե ձեր նպատակը նախագիծը դարձնել production-grade շախմատային հարթակ է, ներկայիս ճարտարապետությունն արդեն աջակցում է այդ ուղղությանը.
- ունեք մոդուլային command/query սահմաններ,
- ունեք հստակ real-time փոխազդեցության պայմանագրեր,
- ունեք AI շարժիչ՝ ընդլայնելի գնահատման տրամաբանությամբ։

Վստահելի մասշտաբավորման համար հաջորդ քայլերը պետք է լինեն.
- ավելի ուժեղ deterministic թեստավորում՝ քայլերի օրինականության և թագավորի անվտանգության շուրջ,
- վիճակի վերականգնման ռազմավարություն hub disconnect-ների դեպքում,
- AI-ի արդյունավետության պրոֆիլավորում՝ ըստ խորության և branching factor-ի։

---

## 10) Practical Review Note
## 10) Գործնական review նշում

This document reflects your current implementation logic and product direction based on the existing code structure.  
Use it as a living engineering narrative and update it as rules, AI heuristics, and real-time workflows evolve.

Այս փաստաթուղթը արտացոլում է ձեր ընթացիկ իրականացման տրամաբանությունն ու արտադրանքային ուղղությունը՝ հիմնվելով առկա կոդային կառուցվածքի վրա։  
Օգտագործեք այն որպես «կենդանի» ինժեներական նկարագրություն և թարմացրեք, երբ զարգանան կանոնները, AI heuristic-ները և real-time հոսքերը։
