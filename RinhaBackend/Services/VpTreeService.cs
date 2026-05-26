
using RinhaBackend.Models;

namespace RinhaBackend.Services
{
    public class VpTreeService
    {
        public VpTreeNode[] treeNodes = new VpTreeNode[3000000];

        private float[] _distanciasBuffer = new float[3000000];

        private int _nextNodeId = 0;
        private TransactionRecord[] _dataset; 

        public VpTreeService(DataLoaderService dataLoader)
        {
            _dataset = dataLoader.Dataset;
        }

        public int CreateTree(int start, int end)
        {
            if (start > end)
                return -1;

            if (start == end)
            {
                int leafId = _nextNodeId++;
                treeNodes[leafId] = new VpTreeNode(leafId, -1, -1, 0f);
                return leafId;
            }

            int indexPontoCentral = end;
            TransactionRecord pontoCentral = _dataset[indexPontoCentral];

            int quantidadeDeItens = end - start;

            for (int i = 0; i < quantidadeDeItens; i++)
            {
                _distanciasBuffer[start + i] = CalcularDistancia(_dataset[start + i].Vector, pontoCentral.Vector);
            }

            Array.Sort(_distanciasBuffer, _dataset, start, quantidadeDeItens);

            int meioDaLista = quantidadeDeItens / 2;
            int middleIndex = start + meioDaLista;
            float threshold = _distanciasBuffer[middleIndex]; 

 
            int currentNodeId = _nextNodeId++;
            int idEsquerda = CreateTree(start, middleIndex - 1);
            int idDireita = CreateTree(middleIndex, end - 1);

            treeNodes[currentNodeId] = new VpTreeNode(currentNodeId, idEsquerda, idDireita, threshold);

            return currentNodeId;
        }

        public List<TransactionRecord> Search(sbyte[] targetVector, int k = 5)
        {
            var pq = new PriorityQueue<TransactionRecord, float>(Comparer<float>.Create((a, b) => b.CompareTo(a)));
            SearchRecursive(0, targetVector, pq, k);
            var result = new List<TransactionRecord>();
            while (pq.Count > 0)
            {
                result.Add(pq.Dequeue());
            }
            result.Reverse();
            return result;
        }

        private void SearchRecursive(int nodeId, sbyte[] targetVector, PriorityQueue<TransactionRecord, float> pq, int k)
        {
            if (nodeId == -1) return;

            var node = treeNodes[nodeId];
            var pontoCentral = _dataset[nodeId];

            float dist = CalcularDistancia(pontoCentral.Vector, targetVector);

            float tau = float.MaxValue;
            if (pq.Count == k)
            {
                pq.TryPeek(out _, out tau);
            }

            if (dist < tau || pq.Count < k)
            {
                pq.Enqueue(pontoCentral, dist);

                if (pq.Count > k)
                {
                    pq.Dequeue();
                }
            }

            if (pq.Count == k)
            {
                pq.TryPeek(out _, out tau);
            }

            if (dist < node.Threshold)
            {
                SearchRecursive(node.LeftChildId, targetVector, pq, k);
                if (dist + tau >= node.Threshold)
                {
                    SearchRecursive(node.RightChildId, targetVector, pq, k);
                }
            }
            else
            {
                SearchRecursive(node.RightChildId, targetVector, pq, k);
                if (dist - tau <= node.Threshold)
                {
                    SearchRecursive(node.LeftChildId, targetVector, pq, k);
                }
            }
        }

        private float CalcularDistancia(ReadOnlySpan<sbyte> a, ReadOnlySpan<sbyte> b)
        {
            float soma = 0f;
            for (int i = 0; i < 14; i++)
            {
                float diff = a[i] - b[i];
                soma += (diff * diff);
            }
            return (float)Math.Sqrt(soma);
        }
    }
}