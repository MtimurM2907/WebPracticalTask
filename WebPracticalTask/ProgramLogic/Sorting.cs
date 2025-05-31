namespace WebPracticalTask.ProgramLogic
{
    public class Sorting
    {
        public static List<int> finalTextIndex = new List<int>();

        //преобразование символов в числа
        public static void SymbolsToNumbers()
        {
            foreach (char j in Logics.finalText)
            {
                for (int i = 0; i < Logics.englishAlphabet.Length; i++)
                {
                    if (j == Logics.englishAlphabet[i])
                    {
                        finalTextIndex.Add(i);
                    }
                }
            }
        }

        //преобразование чисел в символы
        public static void NumbersToSymbols()
        {
            string sortingText = "\0";
            foreach (int j in finalTextIndex)
            {
                for (int i = 0; i < Logics.englishAlphabet.Length; i++)
                {
                    if (j == i)
                    {
                        sortingText += Logics.englishAlphabet[i];
                    }
                }
            }
            Logics.finalText = sortingText;
        }




        public static int FindPivot(int startIndex, int endIndex)
        {
            int pivotIndex = startIndex - 1;
            int temp = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (finalTextIndex[i] < finalTextIndex[endIndex])
                {
                    pivotIndex++;
                    temp = finalTextIndex[i];
                    finalTextIndex[i] = finalTextIndex[pivotIndex];
                    finalTextIndex[pivotIndex] = temp;
                }
            }
            pivotIndex++;
            temp = finalTextIndex[pivotIndex];
            finalTextIndex[pivotIndex] = finalTextIndex[endIndex];
            finalTextIndex[endIndex] = temp;

            return pivotIndex;
        }

        public static void QuicksortLogic(int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return;
            }
            int pivot = FindPivot(startIndex, endIndex);
            QuicksortLogic(startIndex, pivot - 1);
            QuicksortLogic(pivot + 1, endIndex);
        }

        //быстрая сортировка
        public static void Quicksort()
        {
            SymbolsToNumbers();
            QuicksortLogic(0, finalTextIndex.Count - 1);
            NumbersToSymbols();
        }




        public static TreeNode Insert(TreeNode root, int value)
        {
            if (root == null)
                return new TreeNode(value);

            if (value < root.Value)
                root.Left = Insert(root.Left, value);
            else
                root.Right = Insert(root.Right, value);

            return root;
        }

        public static void InOrderTraversal(TreeNode root, List<int> result)
        {
            if (root != null)
            {
                InOrderTraversal(root.Left, result);
                result.Add(root.Value);
                InOrderTraversal(root.Right, result);
            }
        }

        //сортировка деревом
        public static void TreeSort()
        {
            SymbolsToNumbers();
            TreeNode root = null;
            foreach (var i in finalTextIndex)
            {
                root = Insert(root, i);
            }

            var result = new List<int>();
            InOrderTraversal(root, result);
            for (int i = 0; i < finalTextIndex.Count; i++)
            {
                finalTextIndex[i] = result[i];
            }
            NumbersToSymbols();
        }
    }
}
