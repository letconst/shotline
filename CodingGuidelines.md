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
    * 例:<br>public void DoSomething()
* private変数は先頭にアンダースコア (`_`) を設け、キャメルケースとする
    * 例:<br>private int _num;<br>private static float _playerSpeed;
* public変数はキャメルケースを使用する
    * 例:<br>public int num;<br>public float playerSpeed;
* public static変数および定数はパスカルケースを使用する
    * 例:<br>public static int Num;<br>public const float PlayerSpeed;
* プロパティはパスカルケースを使用する
    * 例:<br>public int Num { get; set; }
* 配列やListは複数形を使用する
    * 例:<br>public int[] nums;<br>public List\<string\> itemNames;
* ローカル変数はキャメルケースを使用する
    * 例:<br>int num;
* ループ変数は`i`, `j`, `k` ...を使用する
    * 例:<br>for (int i; i < 10; i++)
* 抽象クラスはパスカルケースを使用し、末尾に `Base` を付ける
    * 例:<br>public abstract class ItemBase
* インターフェイスはパスカルケースを使用し、先頭に `I` を付ける
    * 例:<br>public interface IAttackable
* 構造体および列挙体はパスカルケースを使用する
    * 例:<br>public struct Player<br>public enum CurrentPhase
