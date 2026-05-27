using RinhaBackend.Models;
using System.Runtime.CompilerServices;

namespace RinhaBackend.Services
{
    public class VpTreeService
    {
        public VpTreeNode[] treeNodes = new VpTreeNode[3000000];
        private float[]? _distanciasBuffer;
        private int _nextNodeId = 0;
        private TransactionRecord[] _dataset; 

        public VpTreeService(DataLoaderService dataLoader)
        {
            _dataset = dataLoader.Dataset;
        }

        public int Build()
        {
            _distanciasBuffer = new float[3000000];
            int root = CreateTree(0, _dataset.Length - 1);
            
            _distanciasBuffer = null; 
            GC.Collect(); 
            
            return root;
        }

        private int CreateTree(int start, int end)
        {
            if (start > end) return -1;

            if (start == end)
            {
                int leafId = _nextNodeId++;
                treeNodes[leafId] = new VpTreeNode(-1, -1, 0f);
                return leafId;
            }

            int indexPontoCentral = end;
            TransactionRecord pontoCentral = _dataset[indexPontoCentral];
            int quantidadeDeItens = end - start;

            for (int i = 0; i < quantidadeDeItens; i++)
            {
                _distanciasBuffer![start + i] = CalcularDistancia(_dataset[start + i].Vector, pontoCentral.Vector);
            }

            Array.Sort(_distanciasBuffer, _dataset, start, quantidadeDeItens);

            int meioDaLista = quantidadeDeItens / 2;
            int middleIndex = start + meioDaLista;
            float threshold = _distanciasBuffer![middleIndex]; 

            int currentNodeId = _nextNodeId++;
            int idEsquerda = CreateTree(start, middleIndex - 1);
            int idDireita = CreateTree(middleIndex, end - 1);

            treeNodes[currentNodeId] = new VpTreeNode(idEsquerda, idDireita, threshold);
            return currentNodeId;
        }

        public List<TransactionRecord> Search(sbyte[] targetVector, int k = 5)
        {
            var pq = new PriorityQueue<TransactionRecord, float>(k);
            int nosVisitados = 0; 

            SearchRecursive(0, targetVector, pq, k, ref nosVisitados); 
            
            var result = new List<TransactionRecord>();
            while (pq.Count > 0)
            {
                result.Add(pq.Dequeue());
            }
            result.Reverse();
            return result;
        }

        private void SearchRecursive(int nodeId, sbyte[] targetVector, PriorityQueue<TransactionRecord, float> pq, int k, ref int nosVisitados)
        {
            if (nodeId == -1 || nosVisitados >= 600) return;

            nosVisitados++; 

            var node = treeNodes[nodeId];
            var pontoCentral = _dataset[nodeId];

            float dist = CalcularDistancia(pontoCentral.Vector, targetVector);

            float tau = float.MaxValue;
            if (pq.Count == k)
            {
                pq.TryPeek(out _, out float topPriority);
                tau = -topPriority;
            }

            if (dist < tau || pq.Count < k)
            {
                pq.Enqueue(pontoCentral, -dist);
                if (pq.Count > k)
                {
                    pq.Dequeue();
                }
            }

            if (pq.Count == k)
            {
                pq.TryPeek(out _, out float topPriority);
                tau = -topPriority;
            }

            if (dist < node.Threshold)
            {
                SearchRecursive(node.LeftChildId, targetVector, pq, k, ref nosVisitados);
                if (dist + tau >= node.Threshold)
                {
                    SearchRecursive(node.RightChildId, targetVector, pq, k, ref nosVisitados);
                }
            }
            else
            {
                SearchRecursive(node.RightChildId, targetVector, pq, k, ref nosVisitados);
                if (dist - tau <= node.Threshold)
                {
                    SearchRecursive(node.LeftChildId, targetVector, pq, k, ref nosVisitados);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float CalcularDistancia(ReadOnlySpan<sbyte> a, ReadOnlySpan<sbyte> b)
        {
            int d0 = a[0] - b[0];
            int d1 = a[1] - b[1];
            int d2 = a[2] - b[2];
            int d3 = a[3] - b[3];
            int d4 = a[4] - b[4];
            int d5 = a[5] - b[5];
            int d6 = a[6] - b[6];
            int d7 = a[7] - b[7];
            int d8 = a[8] - b[8];
            int d9 = a[9] - b[9];
            int d10 = a[10] - b[10];
            int d11 = a[11] - b[11];
            int d12 = a[12] - b[12];
            int d13 = a[13] - b[13];

            float soma = (d0*d0) + (d1*d1) + (d2*d2) + (d3*d3) + 
                         (d4*d4) + (d5*d5) + (d6*d6) + (d7*d7) + 
                         (d8*d8) + (d9*d9) + (d10*d10) + (d11*d11) + 
                         (d12*d12) + (d13*d13);

            return (float)Math.Sqrt(soma);
        }
    }
}