using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArabicSupport;

public class QuizManager : MonoBehaviour
{
    public List<QandA> QnA = new List<QandA>(); // Initialize the list
    public GameObject[] options;
    public int currentQuestion;
    public Text QuestionText,D;

    // Singleton instance
    public static QuizManager Instance;

    public bool isCorrect = false;

 // Audio clips for feedback
    public AudioClip correctAudio;
    public AudioClip wrongAudio;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Ensure an AudioSource component is present
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource component was missing and has been added automatically.");
        }

        // Load audio clips programmatically if not assigned
        if (correctAudio == null)
        {
            correctAudio = Resources.Load<AudioClip>("Correct");
            if (correctAudio == null) Debug.LogError("Correct.mp3 is missing in Resources folder!");
        }

        if (wrongAudio == null)
        {
            wrongAudio = Resources.Load<AudioClip>("Wrong");
            if (wrongAudio == null) Debug.LogError("Wrong.mp3 is missing in Resources folder!");
        }

        if (QuestionText == null)
        {
            Debug.LogError("QuestionText is not assigned in the Inspector!");
            return;
        }

        if (options == null || options.Length == 0)
        {
            Debug.LogError("Options array is not assigned or empty in the Inspector!");
            return;
        }

        InitializeQuestions(); // Initialize questions and answers
        GenerateQuestion();
    }

    public void PlayFeedbackAudio(bool isCorrect)
    {
        if (audioSource == null) return;

        AudioClip clip = isCorrect ? correctAudio : wrongAudio;
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("Audio clip for feedback is not assigned!");
        }
    }

    public void Correct()
    {
        if (currentQuestion < 0 || currentQuestion >= QnA.Count)
        {
            Debug.LogError("Invalid question index during Correct()!");
            return;
        }

        QnA.RemoveAt(currentQuestion);
        if (QnA.Count > 0)
        {
            GenerateQuestion();
        }
        else
        {
            Debug.Log("Quiz Complete!");
            Application.Quit();
        }
    }

    void SetAnswers()
    {
        if (options == null || options.Length == 0)
        {
            Debug.LogError("Options array is not set or empty!");
            return;
        }

        for (int i = 0; i < options.Length; i++)
        {
            if (options[i] == null)
            {
                Debug.LogError($"Option {i} is not assigned in the Inspector!");
                continue;
            }

            var answerComponent = options[i].GetComponent<Answer>();
            if (answerComponent == null)
            {
                Debug.LogError($"Option {i} is missing the Answer script!");
                continue;
            }

            // Reset all options to not correct
            answerComponent.isCorrect = false;

            if (i < QnA[currentQuestion].Answers.Length)
            {
                var answerText = options[i].transform.GetChild(0).GetComponent<Text>();
                if (answerText == null)
                {
                    Debug.LogError($"Option {i} is missing a child Text component!");
                    continue;
                }

                // Set the answer text for this option
                answerText.text = ArabicFixer.Fix(QnA[currentQuestion].Answers[i]);
                Debug.Log($"Option {i} text set to: {QnA[currentQuestion].Answers[i]}");

                // Mark the correct answer
                if (QnA[currentQuestion].CorrectAnswer == i ) // CorrectAnswer is 1-based index
                {
                    answerComponent.isCorrect = true;
                    Debug.Log($"Option {i} is marked as correct.");
                }
            }
            else
            {
                Debug.LogWarning($"Not enough answers for question {currentQuestion}. Disabling option {i}.");
                options[i].SetActive(false); // Hide extra options
            }
        }
    }

    void GenerateQuestion()
    {
        if (QnA == null || QnA.Count == 0)
        {
            Debug.LogError("No questions available to generate!");
            return;
        }

        currentQuestion = Random.Range(0, QnA.Count);

        if (QuestionText == null)
        {
            Debug.LogError("QuestionText is not assigned!");
            return;
        }

        QuestionText.text = ArabicFixer.Fix(QnA[currentQuestion].Question);
        D.text=ArabicFixer.Fix(QnA[currentQuestion].D);

        SetAnswers();
    }

    // Initialize questions and answers directly in the script
    void InitializeQuestions()
    {
        if (QnA == null)
        {
            QnA = new List<QandA>();
        }
        //Easy
        QnA.Add(new QandA
        {
            Question = "ما هي الليلة التي أنزل فيها القرآن؟",
            Answers = new string[] { "الثلاثاء", "الجمعة", "القدر" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو أول ما يسأل عنه المرء؟",
            Answers = new string[] { "الصلاة", "الزكاة", "بر الوالدين" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أركان الإسلام؟",
            Answers = new string[] { "أربعة", "خمسة", "ستة" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "أين وُلد رسول الله صلى الله عليه وسلم؟",
            Answers = new string[] { "مكة المكرمة", "المدينة المنورة", "تبوك" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي أعلى درجات الجنة؟",
            Answers = new string[] { "الخلد", "الفردوس", "النعيم" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من هي مرضعة رسول الله صلى الله عليه وسلم؟",
            Answers = new string[] { "خديجة بنت خويلد", "حليمة السعدية", "آمنة بنت وهب" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي الصلاة التي لا ركوع فيها ولا سجود؟",
            Answers = new string[] { "الجنازة", "الاستسقاء", "الجمعة" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "كم مرة قام رسول الله صلى الله عليه وسلم بمناسك الحج؟",
            Answers = new string[] { "مرة واحدة", "مرتين", "ثلاث مرات" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أجزاء القرآن الكريم؟",
            Answers = new string[] { "خمسة عشر", "ثلاثين", "ستين" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو أول شيء يقوم به الشخص عند الحج؟",
            Answers = new string[] { "الإحرام", "القدوم", "السعي" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من هو أول من يدخل الجنة؟",
            Answers = new string[] { "آدم عليه السلام", "محمد صلى الله عليه وسلم", "عيسى عليه السلام" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من الذي دفن آدم عليه السلام؟",
            Answers = new string[] { "حواء", "الملائكة", "ذريته" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة التي تسمى 'عروس القرآن'؟",
            Answers = new string[] { "البقرة", "الرحمن", "الفاتحة" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أركان الإيمان؟",
            Answers = new string[] { "أربعة", "خمسة", "ستة" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو أول مسجد بني في الإسلام؟",
            Answers = new string[] { "الحرم", "الأقصى", "قباء" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الذي يأخذ أجر حضور الجمعة وصلاة الجمعة وهو جالس في بيته؟",
            Answers = new string[] { "المرأة", "اليتيم", "الكفيف" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "في أي عام شرع شهر رمضان المبارك؟",
            Answers = new string[] { "ثمانية", "أحد عشر", "اثنين" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "متى لقب أبو بكر الصديق بهذا الاسم؟",
            Answers = new string[] { "في حادثة الإفك", "ليلة الإسراء والمعراج", "في أحداث غار حراء" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الملقب بالصديق؟",
            Answers = new string[] { "إسحاق", "يعقوب", "يوسف" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة التي تعدل ثلث القرآن الكريم؟",
            Answers = new string[] { "الإخلاص", "البقرة", "النساء" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو الحيوان الصنم الذي عبده بنو إسرائيل؟",
            Answers = new string[] { "الخنزير", "العجل", "القرد" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي لم يؤمن به أحد من قومه؟",
            Answers = new string[] { "نوح", "لوط", "هود" },
            CorrectAnswer = 0,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي الحشرة التي ضرب الله بها مثلاً في القرآن الكريم؟",
            Answers = new string[] { "الفراشة", "النحلة", "البعوضة" },
            CorrectAnswer = 2,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "كم عاش النبي صلى الله عليه وسلم؟",
            Answers = new string[] { "ستين", "ثلاث وستين", "خمسة وستين" },
            CorrectAnswer = 1,
            D="سهل"
        });

        QnA.Add(new QandA
        {
            Question = "أين توفي النبي صلى الله عليه وسلم؟",
            Answers = new string[] { "المدينة المنورة", "مكة المكرمة", "تبوك" },
            CorrectAnswer = 0,
            D="سهل"
        });
        //Middel
        QnA.Add(new QandA
        {
            Question = "ما اسم النبي الذي يتحدث ويفهم لغة الحيوانات؟",
            Answers = new string[] { "لوط (عليه السلام)", "سليمان (عليه السلام)", "إبراهيم (عليه السلام)" },
            CorrectAnswer = 1,
             D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من النبي الذي ابتلعه الحوت؟",
            Answers = new string[] { "يونس (عليه السلام)", "إبراهيم (عليه السلام)", "سليمان (عليه السلام)" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد سور القرآن الكريم؟",
            Answers = new string[] { "مائة وثلاثة عشر", "مائة وأربعة عشر", "مائة وخمسة عشر" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة التي لا تبدأ بالبسملة؟",
            Answers = new string[] { "التوبة", "آل عمران", "النمل" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "في أي سورة توجد آية الكرسي؟",
            Answers = new string[] { "الكهف", "البقرة", "آل عمران" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي الغزوة التي جرح فيها رسول الله صلى الله عليه وسلم وشج رأسه؟",
            Answers = new string[] { "بدر", "أُحد", "تبوك" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي كانت زوجته كافرة ووقفت إلى جانب قومها؟",
            Answers = new string[] { "هود", "نوح", "لوط" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي راودته امرأة التي تربى في بيتها؟",
            Answers = new string[] { "نوح", "يعقوب", "يوسف" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "في أي يوم خرج نبي الله آدم عليه السلام من الجنة؟",
            Answers = new string[] { "الإثنين", "السبت", "الجمعة" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي كان قومه ينحتون من الجبال بيوتاً؟",
            Answers = new string[] { "هود", "صالح", "لوط" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أبناء رسول الله صلى الله عليه وسلم؟",
            Answers = new string[] { "ثمانية", "خمسة", "سبعة" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي كفل السيدة مريم؟",
            Answers = new string[] { "زكريا", "يحيى", "إسماعيل" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد بنات رسول الله محمد صلى الله عليه وسلم؟",
            Answers = new string[] { "ثلاثة", "أربعة", "خمسة" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما السورة التي انتهت باسم وقت من أوقات الصلاة؟",
            Answers = new string[] { "الفجر", "الضحى", "القدر" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي أنجب بعد مشيبته؟",
            Answers = new string[] { "إبراهيم (عليه السلام)", "يحيى (عليه السلام)", "زكريا (عليه السلام)" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم مرة ذكر اسم جبريل في القرآن الكريم؟",
            Answers = new string[] { "ستة", "خمسة", "ثلاثة" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما الاسم الذي أطلق على سورة التوبة؟",
            Answers = new string[] { "الفاضحة", "الفرائض", "التوديع" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي فقد كل شيء ثم رده الله إليه؟",
            Answers = new string[] { "عيسى", "أيوب", "يونس" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هي أول شهيدة في الإسلام؟",
            Answers = new string[] { "سمية بنت الخياط", "خديجة بنت خويلد", "عائشة بنت أبي بكر" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي الشجرة التي تشبه الشيطان؟",
            Answers = new string[] { "الزقوم", "الخلد", "الغرقد" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي نقل بعد موته من مصر إلى فلسطين؟",
            Answers = new string[] { "إسحاق", "موسى", "يوسف" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي أولى السبع الموبقات؟",
            Answers = new string[] { "الربا", "قتل النفس", "الشرك" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم كان عمر النبي عندما هاجر إلى المدينة المنورة؟",
            Answers = new string[] { "واحد وخمسين", "ثلاثة وخمسين", "تسعة وأربعين" },
            CorrectAnswer = 1,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو الطعام الذي أنزل من السماء لبني إسرائيل؟",
            Answers = new string[] { "المن والسلوى", "العسل والماء", "التمر والحليب" },
            CorrectAnswer = 0,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم كان عمر نبي الله محمد صلى الله عليه وسلم عندما تزوج عائشة رضي الله عنها؟",
            Answers = new string[] { "أربعة وخمسين", "خمسة وخمسين", "ستة وخمسين" },
            CorrectAnswer = 2,
            D="متوسط"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أبناء نبي الله محمد صلى الله عليه وسلم؟",
            Answers = new string[] { "ثلاثة", "أربعة", "خمسة" },
            CorrectAnswer = 0,
            D="متوسط"
        });
        //Hard
        QnA.Add(new QandA
        {
            Question = "ما اسم النبي الذي تكرر في القرآن الكريم؟",
            Answers = new string[] { "إسماعيل (عليه السلام)", "موسى (عليه السلام)", "يوسف (عليه السلام)" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد السنين التي نامها أهل الكهف؟",
            Answers = new string[] { "ثلاثمائة", "ثلاثمائة وثلاثة", "ثلاثمائة وتسعة" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أبواب النار؟",
            Answers = new string[] { "ستة", "سبعة", "ثمانية" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أبواب الجنة؟",
            Answers = new string[] { "ستة", "سبعة", "ثمانية" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة التي تكون آخر كلمة فيها هي اسم السورة نفسها؟",
            Answers = new string[] { "الفلق", "الماعون", "الناس" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الصحابي الذي قرأ القرآن في ركعة واحدة؟",
            Answers = new string[] { "الوليد بن المغيرة", "عثمان بن عفان", "بلال بن رباح" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الصحابي الذي شيعه سبعون ألف ملك؟",
            Answers = new string[] { "سعد بن أبي وقاص", "سعد بن معاذ", "عبدالرحمن بن عوف" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الصحابي الملقب بأمين هذه الأمة؟",
            Answers = new string[] { "أبو عبيدة بن الجراح", "حذيفة بن اليمان", "خالد بن الوليد" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو أول طعام يأكله أهل الجنة؟",
            Answers = new string[] { "التمر", "كبد الحوت", "الكبش" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي زوج ابنته لنبي؟",
            Answers = new string[] { "إسحاق", "زكريا", "شعيب" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد الأنبياء الذين ذكروا في القرآن الكريم؟",
            Answers = new string[] { "ثلاثة وعشرين", "خمسة وعشرين", "ستة وعشرين" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما الشيء الذي ذكر في القرآن مرة ككذبة ومرة كدليل؟",
            Answers = new string[] { "هدهد سليمان", "صوت الحمير", "قميص يوسف" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الصحابي الذي غسلته الملائكة؟",
            Answers = new string[] { "سعد بن معاذ", "عمار بن ياسر", "حنظلة بن أبي عامر" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي القصة التي كانت دليلاً على نبوة نبي الله محمد صلى الله عليه وسلم؟",
            Answers = new string[] { "انشقاق القمر", "أصحاب السبت", "الإسراء والمعراج" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو أول من دفن في البقيع مع الصحابة؟",
            Answers = new string[] { "عثمان بن مظعون", "عبيدالله بن الحارث", "أبو أيوب الأنصاري" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو الصحابي الذي لقب بسيد القراء؟",
            Answers = new string[] { "عبد الله بن مسعود", "سعد بن معاذ", "أبي بن كعب" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي صلاة الأوابين؟",
            Answers = new string[] { "الوتر", "الجمعة", "الضحى" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي أنزل الله عليه جراد من ذهب؟",
            Answers = new string[] { "يونس", "أيوب", "نوح" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من هو النبي الذي قبضت روحه وهو ساجد؟",
            Answers = new string[] { "إدريس", "إبراهيم", "صالح" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ماذا كانت تعبد الملكة بلقيس وقومها من دون الله؟",
            Answers = new string[] { "الأصنام", "الشمس", "العجل" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو الحيوان الذي أماته الله ثم أحياه؟",
            Answers = new string[] { "الحمار", "البقرة", "الكلب" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هو الاسم الذي أطلق على جبل عرفات؟",
            Answers = new string[] { "الجودي", "الطور", "الرحمة" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة التي لم يذكر بها لفظ 'الجنة'؟",
            Answers = new string[] { "محمد", "يوسف", "المعارج" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما معنى كلمة 'لغوب' في القرآن الكريم؟",
            Answers = new string[] { "التعب", "اللعب", "السحر" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "من النبي الذي عندما مات كادت أن تقوم القيامة؟",
            Answers = new string[] { "موسى", "محمد", "يحيى" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي السورة القرآنية التي يبكي الشيطان عند سماعها؟",
            Answers = new string[] { "الفاتحة", "الكهف", "السجدة" },
            CorrectAnswer = 2,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "ما هي الشجرة التي أشار إليها تنبت في طور سيناء؟",
            Answers = new string[] { "النخيل", "الزيتون", "الرمان" },
            CorrectAnswer = 1,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد أجنحة جبريل (عليه السلام)؟",
            Answers = new string[] { "ستمائة جناح", "ألف جناح", "مائتي جناح" },
            CorrectAnswer = 0,
            D="صعب"
        });

        QnA.Add(new QandA
        {
            Question = "كم عدد السور في القرآن الكريم التي سُميت بأسماء الرسل؟",
            Answers = new string[] { "إحدى عشر", "ستة", "ثمانية" },
            CorrectAnswer = 1,
            D="صعب"
        });
        Debug.Log("Questions Initialized");
    }
}
