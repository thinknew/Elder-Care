using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Conversive.Verbot5;


namespace ElderCare
{
    class MyPipeline : UtilMPipeline
    {
        protected int nframes;
        protected bool device_lost;

        public MyPipeline()
            : base()
        {
            EnableVoiceRecognition();
            nframes = 0;
            device_lost = false;
        }
        public override bool OnDisconnect()
        {
            if (!device_lost) Console.WriteLine("Device disconnected");
            device_lost = true;
            return base.OnDisconnect();
        }

        public override void OnReconnect()
        {
            Console.WriteLine("Device reconnected");
            device_lost = false;
        }

        public override void OnRecognized(ref PXCMVoiceRecognition.Recognition data)
        {
            Console.WriteLine("\nRecognized<{0}>", data.dictation);
            string sent = data.dictation;
            //Conversation

            Verbot5Engine verbot = new Verbot5Engine();
            KnowledgeBase kb = new KnowledgeBase();
            KnowledgeBaseItem kbi = new KnowledgeBaseItem();
            State state = new State();

            // build the knowledgebase
            Rule vRule = new Rule();
            vRule.Id = kb.GetNewRuleId();

            vRule.AddInput("help", "");
            vRule.AddInput("help please", "");
            vRule.AddOutput("I am sending an alert message now", "", "");
            vRule.AddOutput("Sending alert messages", "", "");
            vRule.AddInput("can u call my son now", "");
            vRule.AddOutput("Yes, I am calling", "", "");
            vRule.AddInput("can u call my son now", "");
            vRule.AddOutput("Yes, I am calling", "", "");
            vRule.AddInput("i m felling pain", "");
            vRule.AddOutput("Is it veyy hard pain, do I ned to call someone", "", "");
            vRule.AddInput("pain is too much", "");
            vRule.AddOutput("Ok, I am calling doctor and your sone now", "", "");
            vRule.AddInput("thank you", "");
            vRule.AddOutput("You are welcome", "", "");

            kb.Rules.Add(vRule);

            // save the knowledgebase
            XMLToolbox xToolbox = new XMLToolbox(typeof(KnowledgeBase));
            xToolbox.SaveXML(kb, @"c:\kbi.vkb");

            // load the knowledgebase item
            kbi.Filename = "kbi.vkb";
            kbi.Fullpath = @"c:\";

            // set the knowledge base for verbot
            verbot.AddKnowledgeBase(kb, kbi);

            state.CurrentKBs.Add(@"c:\kbi.vkb");

            // get input
            Console.WriteLine("Please enter your message");

            while (true)
            {
                string msg = sent;// Console.ReadLine();

                // process the reply
                Reply reply = verbot.GetReply(msg, state);
                if (reply != null)
                    Console.WriteLine(reply.AgentText);
                else
                    Console.WriteLine("No reply found.");
            }
       


        }

        public override bool OnNewFrame()
        {
            Console.Write(".");
            return (++nframes < 50000);
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
             
           MyPipeline pipeline = new MyPipeline();
           Console.Write("Listening");
           if (!pipeline.LoopFrames()) Console.WriteLine("Failed to initialize or stream data");
           pipeline.Dispose();
        

        }
    }
    
           
}
