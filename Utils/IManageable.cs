﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{

	/// <summary>관리 가능한 객체 인터페이스입니다.</summary>
	public interface IManageable
	{
		/// <summary>초기화 되었을 때 호출됩니다.</summary>
		public void OnInitialize();
		/// <summary>해제 되었을 때 호출됩니다.</summary>
		public void OnFinalize();
	}
}
