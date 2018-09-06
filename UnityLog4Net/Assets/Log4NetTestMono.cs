using System.IO;
using log4net.Config;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

namespace log4net.Unity.Tests
{
    public class Log4NetTestMono : MonoBehaviour
    {
        Test _test;

        // Use this for initialization
        void Start() {
            _test = new Test();
            _test.Init();

            InvokeRepeating("_Hello", 1F, 0.1F);

            Invoke("_ChangeUser", 10F);
        }

        void _Hello()
        {
            _test.TestLog4NetConfig();
        }

        void _ChangeUser() {
            _test.ChangeUser();
        }

        private void OnDestroy() {
            _test.OnTearDown();
        }
    }
}