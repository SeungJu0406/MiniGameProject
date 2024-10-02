namespace CustomUtility
{
    namespace IO
    {
        /// <summary>
        /// # Summary
        /// Enumeration to define the types of save formats supported by the data save system.
        /// This enumeration is used to specify the save format when saving or loading data.
        /// 데이터 저장 시스템이 지원하는 저장 형식의 유형을 정의하는 열거형.
        /// 이 열거형은 데이터를 저장하거나 불러올 때 저장 형식을 지정하는 데 사용된다.
        ///
        /// ## Usage:
        /// - Use `SaveType.JSON` to save or load data in JSON format.
        /// - Use `SaveType.BINARY` to save or load data in binary format.
        /// ## 사용 방법:
        /// - `SaveType.JSON`을 사용하여 데이터를 JSON 형식으로 저장하거나 불러온다.
        /// - `SaveType.BINARY`를 사용하여 데이터를 바이너리 형식으로 저장하거나 불러온다.
        ///
        /// ## Notes for Extending:
        /// - If adding a new save type to this enumeration, a corresponding save handler class 
        ///   that inherits from `SaveHandle` must be implemented to handle the specific save format.
        ///   This ensures that the new save type is fully supported by the data save system.
        /// ## 확장 시 주의사항:
        /// - 이 열거형에 새로운 저장 형식을 추가할 경우, 해당 저장 형식을 처리하는 
        ///   `SaveHandle`을 상속받는 전용 저장 핸들러 클래스도 구현해야 한다.
        ///   이를 통해 데이터 저장 시스템에서 새로운 저장 형식이 완벽히 지원되도록 보장한다.
        /// </summary>
        public enum SaveType
        {
            /// <summary>
            /// JSON format for saving data.
            /// 데이터를 저장하기 위한 JSON 형식.
            /// </summary>
            JSON,

            /// <summary>
            /// Binary format for saving data.
            /// 데이터를 저장하기 위한 바이너리 형식.
            /// </summary>
            BINARY,
        }
    }
}