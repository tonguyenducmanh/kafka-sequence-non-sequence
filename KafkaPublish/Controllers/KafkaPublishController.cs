using Confluent.Kafka;
using KafkaCore;
using KafkaModel;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPublish.API.Controllers
{
    /// <summary>
    /// api thực hiện publish message lên kafka
    /// </summary>
    [Route("api/kafka")]
    [ApiController]
    public class KafkaPublishController : ControllerBase
    {
        #region Declare

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo controller
        /// </summary>
        public KafkaPublishController() { }

        #endregion

        #region API

        /// <summary>
        /// test việc publish message lên kafka
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromBody] KafkaMessage message)
        {
            try
            {
                KafkaPublisher publisher = new KafkaPublisher();

                KafkaConfig config = ConfigUtil.CenterConfig.KafkaPublishConfig;
                await publisher.PublishAsync(config, message.Message, message.Sequency);
                return Ok("Publish success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// test việc publish message lên kafka
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("sample-publish")]
        public async Task<IActionResult> SamplePublish()
        {
            try
            {
                KafkaPublisher publisher = new KafkaPublisher();

                KafkaConfig config = ConfigUtil.CenterConfig.KafkaPublishConfig;

                List<KafkaMessage> messages = new List<KafkaMessage>() 
                {
                    new KafkaMessage() { Message = "Là ngày đẹp trời mây xanh nắng vàng", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Anh gọi em để nói chia tay", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Mình từng yêu nhau đến vậy", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Sao chỉ còn lời nói chua cay", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Là ngày anh như chết lặng", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Đưa bàn tay về phía em", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Nhưng em chẳng còn muốn nắm lấy", Sequency = "LOU HOÀNG" },
                    new KafkaMessage() { Message = "Bắc Ninh vốn trọng chữ tình", Sequency = "HOÀ MINZY" },
                    new KafkaMessage() { Message = "Nón quai thao em đợi ở sân đình", Sequency = "HOÀ MINZY" },
                    new KafkaMessage() { Message = "Mấy anh hai quay đầu nhìn cũng đỉnh", Sequency = "HOÀ MINZY" },
                    new KafkaMessage() { Message = "Mà các dân chơi gọi là Bắc Bling-ling (Bling-Bling)", Sequency = "HOÀ MINZY" },
                };

                foreach(KafkaMessage message in messages)
                {
                    await publisher.PublishAsync(config, message.Message, message.Sequency);
                }
                return Ok("Publish success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
