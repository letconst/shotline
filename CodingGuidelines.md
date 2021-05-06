# コーディングに関するガイドライン

## 用語
### パスカルケース
* 単語の先頭文字をすべて大文字に、それ以外は小文字にする記法
* 例: PascalCase
  
### キャメルケース
* 一番先頭を小文字で初め、それ以降の単語の先頭文字を大文字に、それ以外は小文字にする記法
* 例: camelCase

## 命名規則
* スクリプトファイル名はパスカルケースを使用する。したがって、クラス名も同様とする
* メソッドはパスカルケースを使用する
```c#
public void DoSomething()
```
* メソッドの引数はキャメルケースを使用する
```c#
public void DoSomething(bool isActive)
```
* private変数は先頭にアンダースコア (`_`) を設け、キャメルケースとする
```c#
private int _num;
private static float _playerSpeed;
```
* public変数はキャメルケースを使用する
```c#
public int num;
public float playerSpeed;
```
* public static変数および定数はパスカルケースを使用する
```c#
public static int Num;
public const float PlayerSpeed;
```
* プロパティはパスカルケースを使用する
```c#
public int Num { get; set; }
```
* 配列やListは複数形を使用する
```c#
public int[] nums;
public List<string> itemNames;
```
* ローカル変数はキャメルケースを使用する
```c#
public void DoSomething()
{
    int num;
}
```
* ループ変数は`i`, `j`, `k` ...を使用する
```c#
for (int i; i < 10; i++)
```
* 抽象クラスはパスカルケースを使用し、末尾に `Base` を付ける
```c#
public abstract class ItemBase
```
* インターフェイスはパスカルケースを使用し、先頭に `I` を付ける
```c#
public interface IAttackable
```
* 構造体および列挙体はパスカルケースを使用する
```c#
public struct Player
public enum CurrentPhase
```
  
## 記述
* インデントは4文字の半角スペースを使用する
* メンバ変数はクラスの最上部にまとめて定義する
    * 変数の種類（SerializeFieldや修飾子など）によって整理するのは可
* メンバメソッドは、Unityのイベントメソッド（`Update()` など）よりも下に定義する
* メンバメソッドには、内外になるべくコメントを記載する
    * メンバメソッド名の1行上に空行を設け、そこに `/` を3回入力することでドキュメントコメント（メソッドそのものの説明）が残せる
```c#
/// <summary>
/// なんかするやつ
/// </summary>
public void DoSomething()
```
* 変数や一連の処理についてのコメントは、基本的にコードの横ではなく上に残す
    * ただし、見栄えが悪くなる等の問題がある場合、横にスペースが残っていればこの限りではない
```c#
// 地面にレイを飛ばす
private Ray ray = new Ray(hit.transform.posisiotn, -Vector3.up);

private int  hp;     // 残り体力
private bool isDead; // 死んでいるか
```
* メソッドに複数の引数を指定する際は、`,` の直後にスペースを設け、それ以外には挿入しない
```c#
// Good
DoSomething(true, 0);

// Bad
DoSomething( true,0 );
```
* メンバ変数やメンバメソッドは、必ずアクセス修飾子を付ける
```c#
// Good
private int _num;

private void DoSomething()

// Bad
int _num;

void DoSomething()
```
